using HtmlAgilityPack;
using System.Xml.Linq;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        private readonly HttpClient httpClient;

        public HtmlReader()
        {
            this.httpClient = new HttpClient();
        }
        public async Task<SiteModel> GetTurbokResultsAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://turbok.by/search?gender=&gender=&catlist=0&searchText={search}", cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Something wrong");

            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            var siteModel = new SiteModel { Name = "Turbok" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");

            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    CardTemplate cardTemplate = new CardTemplate();
                    cardTemplate.UrlPrefix = "https://turbok.by";
                    cardTemplate.Name = cardNode.SelectSingleNode("div[2]/div[1]/div[1]").InnerText.Trim();
                    cardTemplate.Price = cardNode.SelectSingleNode("div[2]/div[2]/div").InnerText.Trim();
                    cardTemplate.Picture = cardNode.SelectSingleNode("div[1]/a/div/img").Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
                    cardTemplate.CatNumber = cardNode.SelectSingleNode("div[2]/div[1]/div[2]/p[1]").InnerText.Trim();
                    cardTemplate.Status = cardNode.SelectSingleNode("div[2]/div[1]/p").InnerText.Trim();
                    if (cardTemplate.Status == "В наличии")
                        cardTemplate.IsAvailable = true;
                    cardTemplate.CardUrl = cardNode.SelectSingleNode("div[1]/a").Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;

                    siteModel.CardTemplates.Add(cardTemplate);
                }
                siteModel.CardTemplates = siteModel.CardTemplates.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }

        public async Task<SiteModel> GetMagnitResultAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://minskmagnit.by/site_search?search_term={search}", cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Something wrong");

            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            var siteModel = new SiteModel { Name = "Minskmagnit" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/main/div/article/div/section/ul/li");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    CardTemplate cardTemplate = new CardTemplate();
                    cardTemplate.UrlPrefix = "https://minskmagnit.by";
                    cardTemplate.Name = cardNode.SelectSingleNode("div[1]/div[2]/div[1]/a").InnerText.Trim() ?? string.Empty;
                    cardTemplate.Price = cardNode.SelectSingleNode("div/div[2]/div[2]/span").InnerText.Replace("&nbsp;", " ").Trim() ?? string.Empty;
                    cardTemplate.Picture = cardNode.SelectSingleNode("div/div[1]/a/img")?.Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
                    cardTemplate.CatNumber = cardNode.SelectSingleNode("div/div[2]/span/text()")?.InnerText.Trim() ?? string.Empty;
                    cardTemplate.Status = cardNode.SelectSingleNode("div/div[2]/div[3]/span[1]")?.InnerText.Trim() ?? string.Empty;
                    if (cardTemplate.Status == "В наличии")
                        cardTemplate.IsAvailable = true;
                    cardTemplate.CardUrl = cardNode.SelectSingleNode("div/div[2]/div[1]/a").Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;

                    siteModel.CardTemplates.Add(cardTemplate);
                }
            }
            return siteModel;
        }
        public async Task<SiteModel> GetAkvilonResultAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://akvilonavto.by/catalog/?q={search}", cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Something wrong");

            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            var siteModel = new SiteModel { Name = "Akvilon" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"view-showcase\"]/div");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    string name = cardNode.SelectSingleNode("div/div[1]/div[2]/div[1]").InnerText.Trim() ?? string.Empty;
                    CardTemplate cardTemplate = new CardTemplate();
                    cardTemplate.UrlPrefix = "https://akvilonavto.by";
                    
                    cardTemplate.Price = cardNode.SelectSingleNode("div/div[1]/div[3]/div/span/span[2]").InnerText.Trim() ?? string.Empty;
                    cardTemplate.Picture = string.Concat(cardTemplate.UrlPrefix, cardNode.SelectSingleNode("div/div[1]/div[1]/div[1]/a/img")?.Attributes.FirstOrDefault(x => x.Name == "data-src")?.Value) ?? string.Empty;
                    cardTemplate.CatNumber = Splite(ref name) ?? string.Empty;
                    cardTemplate.Name = name;
                    cardTemplate.Status = cardNode.SelectSingleNode("div/div[2]/div[1]/div[1]/div/div/span/span")?.InnerText.Trim() ?? string.Empty;
                    if (cardTemplate.Status != "Нет в наличии" )
                        cardTemplate.IsAvailable = true;
                    cardTemplate.CardUrl = cardNode.SelectSingleNode("div/div[1]/div[1]/div[1]/a").Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;
                    //string s2 = "Сальник МАЗ балансира 2.2-145х175х14 РОСИЧЪ (537-3103037-01)";
                    //string cardNumber = Splite(ref s2);
                    //Console.WriteLine(s2);
                    //Console.WriteLine(cardNumber);
                    siteModel.CardTemplates.Add(cardTemplate);
                }
            }
            return siteModel;
        }

        private static string Splite(ref string cardName)
        {
            int charIndexForTrim = 0;
            int numberOfParentheses = 0;
            for (int i = cardName.Length - 1; i >= 0; i--)
            {
                if (cardName[i] == ')')
                {
                    numberOfParentheses++;
                }
                if (cardName[i] == '(')
                    numberOfParentheses--;
                if (numberOfParentheses == 0)
                {
                    charIndexForTrim = i;
                    break;
                }
            }
            var cardNumber = cardName.Remove(cardName.Length - 1)[(charIndexForTrim + 1)..].TrimEnd();
            cardName = cardName.Substring(0, charIndexForTrim).Trim();
            return cardNumber;
        }
    }
}
