namespace SalesOrderProcessing.Models
{
    public class SalesOrder
    {
        public IFormFile uploadedFile { get; set; }
        public string successMessage { get; set; }
        public string SOFileName { get; set; }
        public string SOData { get; set; }
        public string ackFile { get; set; }
        public string ackData { get; set; }
    }
}
