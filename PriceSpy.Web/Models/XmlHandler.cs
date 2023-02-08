using System.Globalization;
using System.Xml;

namespace PriceSpy.Web.Models
{
    public class XmlHandler
    {
        public static void Read(SampleViewModel allShippers)
        {
            string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory + "Prices");
            DirectoryInfo fileList = new DirectoryInfo(pathWithPrices);

            if (fileList.Exists)
            foreach (FileInfo file in fileList.GetFiles("*.xml"))
            {
                Shipper shipper = new Shipper(file.FullName, file.Name.Substring(0, file.Name.Length - 6));
                    if (file.Name.Contains("РФ")) shipper.IsRub = true;
                Console.WriteLine("Загружен файл " + shipper.Name);
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
                    shipper.Elements.Add(element);
                }
                allShippers.shippers.Add(shipper);
            }
            /// else Not Files Found or Not found dirrectory
        }


        public static void Search(SampleViewModel allShippers, string searchQuery)
        {
            
            SampleViewModel.TotalCount = 0;
            foreach (Shipper shipper in allShippers.shippers)
            {
                Console.WriteLine("Выполняется поиск в " + shipper.Name);

                var findElements = from item in shipper.Elements
                                   where item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || item.CatNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                   select item;
                shipper.SelectedElements = findElements.ToList();
                SampleViewModel.TotalCount += shipper.SelectedElements.Count;
            }
        }
    }
}
