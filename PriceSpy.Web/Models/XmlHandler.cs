using System.Globalization;
using System.Xml;

namespace PriceSpy.Web.Models
{
    public class XmlHandler
    {
        public static void Load()
        {
            DirectoryInfo fileList = new DirectoryInfo(DataFromLocalFiles.pathWithPrices);
            SampleViewModel.Shippers.Clear();
            if (fileList.Exists)
            {
                if (fileList.GetFiles("*.xml").Length == 0)
                    SampleViewModel.TextInfo = "Prices not found";

                foreach (FileInfo file in fileList.GetFiles("*.xml"))
                {
                    string priceName = file.Name;
                    Shipper shipper = new Shipper(file.FullName, priceName);
                    DataFromLocalFiles.PriceNameHandler(shipper, priceName);


                    try
                    {
                        float taxes = 1.2f;
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
                            if (shipper.NotContainsTaxes) price *= taxes;
                            element.Price = price;
                            shipper.AllElements.Add(element);
                        }

                    }
                    catch 
                    {
                        shipper.Error = true;
                    }
                    finally
                    {
                        
                        SampleViewModel.Shippers.Add(shipper);
                        string loadStatus = "Loaded";
                        if (shipper.Error)
                            loadStatus = "Read error";
                        Console.WriteLine(shipper.Name + " " + loadStatus);
                        
                    }

                }
                SampleViewModel.Shippers = SampleViewModel.Shippers.OrderByDescending(x => x.IsRub).ToList();
            }
            else SampleViewModel.TextInfo = "Directory /Prices not found";
        }
        public static void Search(string searchQuery)
        {
            SampleViewModel.TotalCount = 0;
            if (SampleViewModel.Shippers.Count == 0) Load();
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