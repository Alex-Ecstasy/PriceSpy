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
        public string Name { get; set; }
        public string PriceFile { get; set; }
        public bool IsRub { get; set; }
        public bool NotContainsTaxes { get; set; }
        public bool Error { get; set; }
        public static int IndexForAccordion { get; set; }
        public ICollection<Element> AllElements { get; set; } = new List<Element>();
        public ICollection<Element> SelectedElements { get; set; } = new List<Element>();
        public int SelectedElementsCount
        {
            get { return SelectedElements.Count; }
        }
        public Shipper(string path, string name)
        {
            PriceFile = path;
            Name = name;
        }
    }
}