using static System.Net.Mime.MediaTypeNames;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Text.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PriceSpy.Web.Models
{
    public static class RateHandler
    {
        private static string pathWithPrices = (AppDomain.CurrentDomain.BaseDirectory);
        private static string File = "/Currency.txt";
        private static string pathExchangeRates = pathWithPrices + File;
        public static async Task CheckRateAsync(string rate)
        {
            if (!string.IsNullOrEmpty(rate)) rate = rate.Replace(".", ",");
            if (!float.TryParse(rate, out float rateExchange) || rate == "0")
            {
                await GetRateFromBank();
            }
            else
            {
                SampleViewModel.Rate = rateExchange;
            }
            if (RateShort.RateForCompare != SampleViewModel.Rate) WriteRateInFile();
            RateShort.RateForCompare = SampleViewModel.Rate;
        }
        public static async Task GetRateFromFileAsync()
        {
            FileInfo fileInf = new FileInfo(pathExchangeRates);
            if (fileInf.Exists)
            {
                StreamReader streamReader = new StreamReader(pathExchangeRates);

                string? rate = streamReader.ReadLine();
                streamReader.Close();

                if (!string.IsNullOrEmpty(rate)) rate = rate.Replace(".", ",");
                if (!float.TryParse(rate, out float rateExchange) || rate == "0")
                {
                    await GetRateFromBank();
                }
                else
                {
                    Console.WriteLine("Rate from File: " + rateExchange);
                    SampleViewModel.Rate = rateExchange;
                }

            }
            else
            {
                await GetRateFromBank();
                WriteRateInFile();
            }
            RateShort.RateForCompare = SampleViewModel.Rate;
        }

        public static async Task GetRateFromBank()
        {
            try
            {
                HttpClient httpClient = new();
                var result = await httpClient.GetAsync("https://api.nbrb.by/exrates/rates/RUB?parammode=2");
                var htmlResult = await result.Content.ReadAsStringAsync();

                RateShort? rateShort = JsonSerializer.Deserialize<RateShort>(htmlResult);
                float rate = rateShort.Cur_OfficialRate / rateShort.Cur_Scale;
                SampleViewModel.Rate = (float)Math.Round(rate, 4);

                Console.WriteLine("Rate from Bank: " + SampleViewModel.Rate);
                //WriteRateInFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                SampleViewModel.Rate = 1;
            }
            
        }
        public static void WriteRateInFile()
        {
            using (StreamWriter writer = new StreamWriter(pathExchangeRates))
            {
                writer.Write(SampleViewModel.Rate);
            }
        }
    }
    public class RateShort
    {
        public float Cur_OfficialRate { get; set; }
        public int Cur_Scale { get; set; }
        public static float RateForCompare { get; set; }
    }
}
