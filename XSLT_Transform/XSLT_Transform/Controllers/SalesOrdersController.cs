using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using XSLT_Transform.Services;

namespace XSLT_Transform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesOrdersController : Controller
    {
        private readonly TransformService _transformService;
        private readonly string _xsltFolder;

        //instantite the gloabl variables
        public SalesOrdersController()
        {
            _transformService = new TransformService();
            _xsltFolder = Path.Combine(Directory.GetCurrentDirectory(), "Xslt");
        }

        #region JSON Input
        [HttpPost("process")]
        public async Task<IActionResult> ProcessSalesOrder(IFormFile jsonFile)
        {
            if (jsonFile == null || jsonFile.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var sReader = new StreamReader(jsonFile.OpenReadStream(), Encoding.UTF8);
            string sInput = await sReader.ReadToEndAsync();

            //gte values for mapping the names
            var format = _transformService.getData(sInput);
            string currentDate = DateTime.Now.ToString("ddMMyyyy");

            //convert to XSLT
            string soXsltPath = Path.Combine(_xsltFolder, "SalesOrderTransform.xslt");
            string ackXsltPath = Path.Combine(_xsltFolder, "SalesOrderAckTransform.xslt");

            string expSO = _transformService.TransformJson(sInput, soXsltPath, "SalesOrder");
            string expAckSO = _transformService.TransformJson(sInput, ackXsltPath, "SalesOrder");

            //match naming cnvention:  orderRef + “-” + categoryCode + “-” + currentDate + “.json”
            string soFile = $"{format.OrderRef}-{format.CategoryCode}-{currentDate}.json";
            //match naming cnvention:  orderRef + “-” + internalRef + “-” + currentDate + “. json”
            string ackFile = $"{format.OrderRef}-{format.InternalRef}-{currentDate}.json";

            //RETRIEVE PROCESSED JSONS
            string saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "processed");

            if (!Directory.Exists(saveFolder)) {
                Directory.CreateDirectory(saveFolder);  
            }

            string soFilePath = Path.Combine(saveFolder, soFile);
            string ackFilePath = Path.Combine(saveFolder,ackFile);

            await System.IO.File.WriteAllTextAsync(soFilePath, expSO, Encoding.UTF8);
            await System.IO.File.WriteAllTextAsync(ackFilePath, expAckSO, Encoding.UTF8);

            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            string soUrl = $"{baseUrl}/processed/{soFile}";
            string ackUrl = $"{baseUrl}/processed/{ackFile}";
            //return the files to show processing was success
            return Ok(new
            {
                Message = "Processed sales order successfully.",
                Outputs = new[]
                {
                    new { FileName = soFile, Url = soUrl},
                    new { FileName = ackFile, Url = ackUrl},
                }
            });
        }
        #endregion

        #region JSON to XML convert on SalesOrder
        //METHDO: takes in the SO.json to convert to XML with xslt implemented
        [HttpPost("convert-so-to-xml")]
        public async Task<IActionResult> ConvertSalesOrderToXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var sReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            string sInput = await sReader.ReadToEndAsync();

            //gte values for mapping the names
            var format = _transformService.getData(sInput);
            string currentDate = DateTime.Now.ToString("ddMMyyyy");

            string soXsltPath = Path.Combine(_xsltFolder, "JsonXmlSOTransform.xslt");
            string xmlResult = _transformService.TransformJson(sInput, soXsltPath, "ExpectedSO");

            //match naming cnvention:  orderRef + “-” + categoryCode + “-” + currentDate + “.json”
            //string xmlFile = $"{format.OrderRef}-{format.CategoryCode}-{currentDate}.json";

            var jsonParsed = Newtonsoft.Json.Linq.JObject.Parse(sInput);
            string orderRef = jsonParsed["orderReference"]?.ToString() ?? "UNKNOWN";
            string category = jsonParsed["category"]?.ToString() ?? "UNKNOWN";

            string xmlFile = $"{orderRef}-{category}-{currentDate}.xml";

            return File(Encoding.UTF8.GetBytes(xmlResult), "application/xml", xmlFile);
        }
        #endregion

        #region JSON to XML convert on SalesOrder Expected
        //METHDO: takes in the SO.json to convert to XML with xslt implemented
        [HttpPost("convert-ack-to-xml")]
        public async Task<IActionResult> ConvertSalesOrderAckToXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            using var sReader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            string sInput = await sReader.ReadToEndAsync();

            //gte values for mapping the names
            var format = _transformService.getData(sInput);
            string currentDate = DateTime.Now.ToString("ddMMyyyy");

            string soXsltPath = Path.Combine(_xsltFolder, "JsonXmlAckTransform.xslt");
            string xmlResult = _transformService.TransformJson(sInput, soXsltPath, "ExpectedSOAck");

            //match naming cnvention:  orderRef + “-” + categoryCode + “-” + currentDate + “.json”
            //string ackFile = $"{format.OrderRef}-{format.InternalRef}-{currentDate}.json";

            var jsonParsed = JObject.Parse(sInput);
            string orderRef = format?.OrderRef ?? jsonParsed["orderReference"]?.ToString() ?? "UNKNOWN";
            string internalRef = format?.InternalRef ?? jsonParsed["internalReference"]?.ToString() ?? "UNKNOWN";

            string ackFile = $"{orderRef}-{internalRef}-{currentDate}.xml";

            return File(Encoding.UTF8.GetBytes(xmlResult), "application/xml", ackFile);
        }
        #endregion
    }
}
