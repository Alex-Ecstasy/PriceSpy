namespace PriceSpy.Web.Models
{
    public class CardTemplate
    {
        public string? Name { get; set; }
        public float? Price { get; set; } = 0;
        public string? Picture { get; set; }
        public string? CatNumber { get; set; }
        public string? Status { get; set; }
        public string? CardUrl { get; set; }
        public bool IsAvailable { get; set; }
        public string? UrlPrefix { get; set; }
    }

    public class Seller
    {
        public string? Name { get; set; }
       
        public ICollection<CardTemplate> CardTemplates { get; set; } = new List<CardTemplate>();

        public int ResultCount
        {
            get
            {
                return CardTemplates.Count;
            }
        }
    }

    public class SampleViewModel
    {
        public ICollection<Seller> Sites { get; set; } = new List<Seller>();
        public static string? Search { get; set;}
        public ICollection<Shipper> shippers { get; set; } = new List<Shipper>();
    }
}