using System.Text.Json;
using System;
using System.IO;

namespace PriceSpy.Web.Models
{
    public static class DataFromLocalFiles
    {
        public static readonly string pathPrices = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Prices");
        public static readonly string pathData = Path.Combine(pathPrices, "Data");
        private static readonly string _sellersNodesFile = "Nodes.json";
        private static readonly string _pathSellersNodesFile = Path.Combine(pathData, _sellersNodesFile);

        public static Shipper PriceNameHandler (Shipper shipper, string priceName)
        {
            if (priceName.Length > 4) priceName = priceName.Substring(0, priceName.Length - 4);
            if (priceName.Contains("ÐÔ", StringComparison.OrdinalIgnoreCase)) shipper.IsRub = true;
            if (priceName.Contains("Áåç ÍÄÑ", StringComparison.OrdinalIgnoreCase)) shipper.NotContainsTaxes = true;
            priceName = priceName.Replace("ÐÁ", "", StringComparison.OrdinalIgnoreCase);
            priceName = priceName.Replace("ÐÔ", "", StringComparison.OrdinalIgnoreCase);
            priceName = priceName.Replace("(Áåç ÍÄÑ)", "", StringComparison.OrdinalIgnoreCase);
            shipper.Name = priceName;
            return shipper;
        }
        public static async Task GetSellersNodesFromFileAsync ()
        {
            FileInfo fileInf = new FileInfo(_pathSellersNodesFile);
            if (fileInf.Exists)
            {
                try
                {
                    string sellersNodesJsonString = await File.ReadAllTextAsync(_pathSellersNodesFile);
                    SampleViewModel.Sellers = JsonSerializer.Deserialize<ICollection<SellersNodes>>(sellersNodesJsonString);
                    //using FileStream fs = new(_pathSellersNodesFile, FileMode.OpenOrCreate);
                    //SampleViewModel.Sellers = (ICollection<SellersNodes>)JsonSerializer.DeserializeAsyncEnumerable<SellersNodes>(fs);
                    foreach (SellersNodes item in SampleViewModel.Sellers)
                    {
                        Console.WriteLine("Nodes " + item?.SiteName + " downloaded");
                    }
                    
                }
                catch
                {
                    Console.WriteLine("Couldn`t load file " + _sellersNodesFile);
                    SiteNodes.SetNodes();
                }


            }
            else
            {
                SiteNodes.SetNodes();
                
            }
        }
        public static async void WriteNodesInFile()
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string sellersNodesJsonString = JsonSerializer.Serialize(SampleViewModel.Sellers, option);
            await File.WriteAllTextAsync(_pathSellersNodesFile, sellersNodesJsonString);
            Console.WriteLine("File saved " + _sellersNodesFile);
        }
        public static void CheckPaths()
        {
            DirectoryInfo pricesDir = new DirectoryInfo(pathPrices);
            if (!pricesDir.Exists) pricesDir.Create();


            DirectoryInfo dataDir = new DirectoryInfo(pathData);
            if (!dataDir.Exists) dataDir.Create();
            
        }
    }
}
