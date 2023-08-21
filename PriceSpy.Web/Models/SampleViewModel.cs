namespace PriceSpy.Web.Models
{
    public class Card
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
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public string Host { get; set; }
       
        public ICollection<Card> CardList { get; set; } = new List<Card>();

        public int ResultCount
        {
            get
            {
                return CardList.Count;
            }
        }
        public Seller(string name, string host)
        {
            Name = name;
            Host = host;
        }
    }

    public class SellerNodes
    {
        public string? NameNode { get; set; }
        public string? PriceNode { get; set; }
        public string? PictureNode { get; set; }
        public string? PictureAttribute { get; set; }
        public string? CatNumberNode { get; set; }
        public string? StatusNode { get; set; }
        public string? CardUrlNode { get; set; }
        public string? UrlPrefixNode { get; set; }
        
    }
    public class SampleViewModel
    {
        public static string? Search { get; set; }
        public static float Rate { get; set; }
        public static int TotalCount { get; set; }
        public static string? TextInfo { get; set; }
        public static ICollection<Shipper> Shippers { get; set; } = new List<Shipper>();
        public static ICollection<Seller> Sites { get; set; } = new List<Seller>();

    }
}