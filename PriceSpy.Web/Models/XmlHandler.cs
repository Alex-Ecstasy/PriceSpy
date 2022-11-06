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


            foreach (FileInfo file in fileList.GetFiles("*.xml"))
            {
                Shipper shipper = new Shipper(file.FullName, file.Name.Substring(0, file.Name.Length - 4));

                Console.WriteLine("Загружен файл " + shipper.Name);
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(shipper.PriceFile);
                XmlNodeList rowList = xdoc.GetElementsByTagName("Row");
                foreach (var docelement in rowList)
                {
                    Element element = new Element();
                    element.CatNumber = (docelement as XmlElement).ChildNodes[0]?.InnerText.Trim();
                    element.Name = (docelement as XmlElement).ChildNodes[1]?.InnerText.Trim();
                    float price = 0;
                    float.TryParse((docelement as XmlElement).ChildNodes[2]?.InnerText.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out price);
                    element.Price = price;
                    shipper.Elements.Add(element);
                }
                allShippers.shippers.Add(shipper);
            }

        }


        public static void Search(SampleViewModel allShippers, string searchQuery)
        {
            //string? search = Console.ReadLine();
            foreach (Shipper shipper in allShippers.shippers)
            {
                Console.WriteLine("Выполняется поиск в " + shipper.Name);

                var findElements = from item in shipper.Elements
                                   where item.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || item.CatNumber.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                                   select item;
                shipper.SelectedElements = findElements.ToList();
            }

            //foreach (var selectedShippers in allShippers.shippers)
            //{
            //    Console.WriteLine("Результаты поиска " + selectedShippers.Name);
            //    {
            //        foreach (var item in selectedShippers.selectedElements)
            //            Console.WriteLine("Название: {0} Номер: {1} Цена с НДС: {2}", item.Name, item.CatNumber, item.Price);
            //    }
            //    Console.WriteLine("******", 40);
            //}
            //Console.WriteLine("******000000********", 50);
        }
    }
}
