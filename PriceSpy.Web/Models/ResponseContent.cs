namespace PriceSpy.Web.Models
{
    public class ResponseContent
    {
        public string? Message { get; set; }
        public string? Status { get; set; }
        public bool isAvailable { get; internal set; }
    }
}
