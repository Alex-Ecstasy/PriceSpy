namespace PriceSpy.Web.Models
{
    public class Element
    {
        public string Name { get; set; } = "Default";
        public string CatNumber { get; set; } = "Default";
        public float? Price { get; set; } = 0;

    }
    public class Shipper
    {
        public string Name { get; private set; }
        public string PriceFile { get; set; }
        public bool IsRub { get; set; }
        public ICollection<Element> Elements { get; set; } = new List<Element>();
        public int ElementsCount
        {
            get { return Elements.Count; }
        }
        public Shipper(string path, string name)
        {
            PriceFile = path;
            Name = name;
        }
    }
}