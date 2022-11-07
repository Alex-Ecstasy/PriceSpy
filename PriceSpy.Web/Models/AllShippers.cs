namespace PriceSpy.Web.Models
{
    public class Element
    {
        public string? Name { get; set; }  = "Default";
        public string? CatNumber { get; set; } = "Default";
        public float? Price { get; set; } = 0;
    }
    public class Shipper
    {
        public string Name { get; set; }
        public int NumberOfElements
        {
            get { return Elements.Count; }
        }
        public ICollection<Element> Elements { get; set; } = new List<Element>();
        public string PriceFile { get; set; }
        public Shipper(string path, string name)
        {
            PriceFile = path;
            Name = name;
        }
        public ICollection<Element> SelectedElements { get; set; } = new List<Element>();
        public int ResultCount
        {
            get { return SelectedElements.Count; }
        }
    }
    public class AllShippers
    {
        //public ICollection<Shipper> shippers { get; set; } = new List<Shipper>();
    }
}