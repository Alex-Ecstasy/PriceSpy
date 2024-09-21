namespace PriceSpy.Web.Models
{
    public static class DataFromLocalFiles
    {
        static string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory);
        public static float GetExchangeRates()
        {
            string File = "/Currency.txt";
            string pathExchangeRates = pathWithPrices + File;
            FileInfo fileInf = new FileInfo(pathExchangeRates);
            if (fileInf.Exists)
            {
                StreamReader streamReader = new StreamReader(pathExchangeRates);

                string? rate = streamReader.ReadLine();
                if (!string.IsNullOrEmpty(rate)) rate = rate.Replace(".", ",");
                if (!float.TryParse(rate, out float rateExchange)) rateExchange = 1;
                Console.WriteLine(rateExchange);
                return rateExchange;
            }
            else return 1;
            
        }
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
    }
}
