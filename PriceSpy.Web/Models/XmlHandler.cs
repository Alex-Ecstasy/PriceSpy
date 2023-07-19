using System.Globalization;
using System.Xml;

namespace PriceSpy.Web.Models
{
    public class XmlHandler
    {
        public static void Load()
        {
            string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory + "Prices");
            DirectoryInfo fileList = new DirectoryInfo(pathWithPrices);
            SampleViewModel.Shippers.Clear();
            if (fileList.Exists)
            {
                if (fileList.GetFiles("*.xml").Length == 0)
                    SampleViewModel.TextInfo = "Prices not found";

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
                        element.Name = (docelement as XmlElement)?.ChildNodes[0]?.InnerText.Trim() ?? string.Empty;
                        element.CatNumber = (docelement as XmlElement)?.ChildNodes[1]?.InnerText.Trim() ?? string.Empty;
                        float price = 0;
                        float.TryParse((docelement as XmlElement)?.ChildNodes[2]?.InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price);
                        //if (shipper.IsRub) price *= SampleViewModel.Rate;
                        element.Price = price;
                        shipper.AllElements.Add(element);
                    }
                    Console.WriteLine("File loaded " + shipper.Name);
                    SampleViewModel.Shippers.Add(shipper);

                }
            }
            else SampleViewModel.TextInfo = "Dirrectory /Prices not found";
        }
        public static void Search(string searchQuery)
        {
            SampleViewModel.TotalCount = 0;
            foreach (var shipper in SampleViewModel.Shippers)
            {
                shipper.SelectedElements.Clear();
                foreach (var item in shipper.AllElements)
                {
                    bool match = item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                         || item.CatNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);
                    if (match)
                    {
                        Element element = new();
                        element.Name = item.Name;
                        element.CatNumber = item.CatNumber;
                        element.Price = item.Price;
                        if (shipper.IsRub) element.Price *= SampleViewModel.Rate;
                        shipper.SelectedElements.Add(element);
                    }

                }
                SampleViewModel.TotalCount += shipper.SelectedElementsCount;
            }
            GC.Collect();
        }
    }
}