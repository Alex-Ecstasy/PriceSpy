namespace PriceSpy.Web.Models
{
    public static class DataFromLocalFiles
    {
        static string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory + "Prices");
        public static float GetExchangeRates()
        {
            string File = "/Currency.txt";
            string pathExchangeRates = pathWithPrices + File;

            StreamReader streamReader = new StreamReader(pathExchangeRates);

            string? rate = streamReader.ReadLine();
            if (!string.IsNullOrEmpty(rate)) rate = rate.Replace(".", ",");
            if (!float.TryParse(rate, out float rateExchange)) rateExchange = 1;
            Console.WriteLine(rateExchange);
            return rateExchange;
        }
    }
}
