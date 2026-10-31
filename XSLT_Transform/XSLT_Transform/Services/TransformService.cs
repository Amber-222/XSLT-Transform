using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XSLT_Transform.Models;

namespace XSLT_Transform.Services
{
    public class TransformService
    {
        //METHOD: takes in a JSON to convert to XML and translate into XSLT format
        public string TransformJson(string iJson, string xsltFilePath, string rootXmlName = "Root")
        {
            //convert uploaded json to xml doc
            XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(iJson, rootXmlName);

            if (xmlDoc == null)
            {
                throw new InvalidOperationException("Failed to convert JSON input to a XML.");
            }

            //pull xslt file
            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltFilePath);

            //pull xslt transofrmation to memorystream for processing
            using var sWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(sWriter, xslt.OutputSettings);

            //convert
            xslt.Transform(xmlDoc, xmlWriter);
            return sWriter.ToString();
        }

        //METHOD: routes and matches json payload to ensure dynamic file naming
        public SalesOrder getData(string jsonData)
        {
            try
            {
                //dynamic obj = JsonConvert.DeserializeObject(jsonData);
                var jsonObj = JObject.Parse(jsonData);

                var obj = jsonObj["SalesOrder"] ?? jsonObj;

                return new SalesOrder
                {
                    OrderRef = obj["orderRef"]?.ToString() ?? obj["orderReference"]?.ToString() ?? "UNKNOWN",
                    CategoryCode = obj["categoryCode"]?.ToString() ?? obj["category"]?.ToString() ?? "UNKNOWN",
                    InternalRef = obj["internalRef"]?.ToString() ?? obj["internalReference"]?.ToString() ?? "UNKNOWN",
                };
            }
            catch
            {
                return new SalesOrder
                {
                    OrderRef = "UNKNOWN",
                    CategoryCode = "UNKNOWN",
                    InternalRef = "UNKNOWN",
                };
            }
        }
    }
}
