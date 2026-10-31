using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SalesOrderProcessing.Models;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.JavaScript;

namespace SalesOrderProcessing.Controllers
{
    public class OrderController : Controller
    {

        //initialise api config
        private readonly HttpClient _httpClient;
        private readonly string _apiURL;

        public OrderController(HttpClient httpClient, IConfiguration config) 
        { 
            _httpClient = httpClient;
            _apiURL = config["ApiSettings:BaseUrl"] ?? "https://localhost:7215/api/SalesOrders";
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new SalesOrder());
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(SalesOrder order)
        {
            if (order.uploadedFile == null ||  order.uploadedFile.Length == 0)
            {
                ModelState.AddModelError("", "Please uplaod a vlaid file.");
                return View("Index", order);
            }

            //connect and pull file from the frontend
            using var data = new MultipartFormDataContent();
            using var fileStream = order.uploadedFile.OpenReadStream();
            using var streamData = new StreamContent(fileStream);
            streamData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            data.Add(streamData, "jsonfile", order.uploadedFile.FileName);

            var response = await _httpClient.PostAsync($"{_apiURL}/process", data); //connect to api endpoint to porcess json file

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(jsonResponse);

                //get data and fields from json fiel
                order.successMessage = json["message"]?.ToString();
                order.SOFileName = json["outputs"]?[0]?["fileName"]?.ToString();
                order.SOData = json["outputs"]?[0]?["content"]?.ToString();
                order.ackFile = json["outputs"]?[1]?["fileName"]?.ToString();
                order.ackData = json["outputs"]?[1]?["content"]?.ToString();
            }
            else
            {
                ModelState.AddModelError("", "Couldn't comm to API.");
            }

            return View("Index", order);
        }

        [HttpPost]
        public async Task<IActionResult> ConvertToXml(IFormFile expectedFile, string apiEndpoint)
        {
            if (expectedFile == null || expectedFile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            //connect and pull file from the frontend
            using var data = new MultipartFormDataContent();
            using var fileStream = expectedFile.OpenReadStream();
            using var streamData = new StreamContent(fileStream);
            streamData.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            data.Add(streamData, "file", expectedFile.FileName);

            var response = await _httpClient.PostAsync($"{_apiURL}/{apiEndpoint}", data); //connect to api endpoint to porcess json file

            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsStreamAsync();
                string fileName = "output.xml";

                if (response.Content.Headers.TryGetValues("Content-Disposition", out var values)) 
                {
                    foreach (var val in values)
                    {
                        var parts = val.Split(';'); //there are 2 filename headers in the request

                        foreach (var part in parts)
                        {
                            var trimmed = part.Trim();
                            //if (part.Trim().StartsWith("filename="))
                            //{
                            //    fileName = part.Trim().Substring("filename=".Length).Replace("\"", "").Trim();
                            //}
                            if (trimmed.StartsWith("filename=", StringComparison.OrdinalIgnoreCase))
                            {
                                fileName = trimmed.Substring("filename=".Length).Trim('"');
                            }
                        }
                    }                    
                }
                return File(bytes, "application/xml", fileName);                    
            }
            return BadRequest("Failed to generate XML file.");
        }
    }
}