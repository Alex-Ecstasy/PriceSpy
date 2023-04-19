using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace PriceSpy.Web.Models
{
    public class XmlHandler
    {
        public static void Read(SampleViewModel allShippers, string searchQuery)
        {
            string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory + "Prices");
            DirectoryInfo fileList = new DirectoryInfo(pathWithPrices);
            SampleViewModel.TotalCount = 0;
                if (fileList.Exists)
            foreach (FileInfo file in fileList.GetFiles("*.xml"))
            {
                Shipper shipper = new Shipper(file.FullName, file.Name.Substring(0, file.Name.Length - 6));
                    if (file.Name.Contains("РФ")) shipper.IsRub = true;
                
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(shipper.PriceFile);
                XmlNodeList rowList = xdoc.GetElementsByTagName("Row");
                foreach (var docelement in rowList)
                {
                    Element element = new();
                    element.CatNumber = (docelement as XmlElement)?.ChildNodes[0]?.InnerText.Trim() ?? string.Empty;
                    element.Name = (docelement as XmlElement)?.ChildNodes[1]?.InnerText.Trim() ?? string.Empty;
                    float price = 0;
                    float.TryParse((docelement as XmlElement)?.ChildNodes[2]?.InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price);
                        if (shipper.IsRub) price *= SampleViewModel.Rate;
                    element.Price = price;
                        bool match = element.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                             || element.CatNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);
                        if (match) shipper.Elements.Add(element);
                }
                    Console.WriteLine("File loaded " + shipper.Name);
                    allShippers.Shippers.Add(shipper);
                    SampleViewModel.TotalCount += shipper.ElementsCount;
                    //xdoc = null;
                    GC.Collect();
                }
            /// else Not Files Found or Not found dirrectory
        }
    }
}