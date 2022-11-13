using HtmlAgilityPack;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        private readonly HttpClient httpClient;
        public HtmlReader()
        {
            this.httpClient = new HttpClient();
        }
        public async Task<Seller> GetTurbokResultsAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://turbok.by/search?gender=&gender=&catlist=0&searchText={search}", cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Turbok wrong");
            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var siteModel = new Seller { Name = "Turbok" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    Card card = new Card();
                    card.UrlPrefix = "https://turbok.by";
                    card.Name = GetName("div[2]/div[1]/div[1]", cardNode);
                    card.Price = GetPrice("div[2]/div[2]/div", cardNode);
                    card.Picture = GetPicture("div[1]/a/div/img", cardNode);
                    card.CatNumber = GetCatNumber("div[2]/div[1]/div[2]/p[1]", cardNode);
                    card.Status = GetStatus("div[2]/div[1]/p", cardNode);
                    if (card.Status == "В наличии") card.IsAvailable = true;
                    card.CardUrl = GetCardUrl("div[1]/a", cardNode);
                    siteModel.CardList.Add(card);
                }
                siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }
        public async Task<Seller> GetMagnitResultAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://minskmagnit.by/site_search?search_term={search}", cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Magnit wrong");
            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var siteModel = new Seller { Name = "Minskmagnit" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/main/div/article/div/section/ul/li");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    Card card = new Card();
                    card.UrlPrefix = "https://minskmagnit.by";
                    card.Name = GetName("div[1]/div[2]/div[1]/a", cardNode);
                    card.Price = GetPrice("div/div[2]/div[2]/span", cardNode);
                    card.Picture = GetPicture("div/div[1]/a/img", cardNode);
                    card.CatNumber = GetCatNumber("div/div[2]/span/text()", cardNode);
                    card.Status = GetStatus("div/div[2]/div[3]/span[1]", cardNode);
                    if (card.Status == "В наличии") card.IsAvailable = true;
                    card.CardUrl = GetCardUrl("div/div[2]/div[1]/a", cardNode);
                    siteModel.CardList.Add(card);
                }
                siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }
        public async Task<Seller> GetAkvilonResultAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://akvilonavto.by/catalog/?q={search}", cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Akvilon wrong");
            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var siteModel = new Seller { Name = "Akvilon" };
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"view-showcase\"]/div");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    Card card = new Card();
                    string name = GetName("div/div[1]/div[2]/div[1]", cardNode);
                    card.UrlPrefix = "https://akvilonavto.by";
                    card.Price = GetPrice("div/div[1]/div[3]/div/span/span[2]", cardNode);
                    card.Picture = GetAkvilonPicture("div/div[1]/div[1]/div[1]/a/img", card.UrlPrefix, cardNode);
                    card.CatNumber = Splite(ref name);
                    card.Name = name;
                    card.Status = GetStatus("div/div[2]/div[1]/div[1]/div/div/span/span", cardNode);
                    if (card.Status != "Нет в наличии" || card.Status != "Неизвестный статус") card.IsAvailable = true;
                    card.CardUrl = GetCardUrl("div/div[1]/div[1]/div[1]/a", cardNode);
                    siteModel.CardList.Add(card);
                }
                siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }
        private static string GetName(string nameNode, HtmlNode cardNode)
        {
            string? cardName = cardNode.SelectSingleNode(nameNode)?.InnerText.Trim().Replace("&#34;", "") ?? string.Empty;
            if (cardName == string.Empty) cardName = "-----";
            return cardName;
        }
        private static float GetPrice(string priceNode, HtmlNode cardNode) /// if Price 10,3 => 10,30 
        {
            float cardPrice = 0;
            var priceText = cardNode.SelectSingleNode(priceNode).InnerText.Trim().Replace("&nbsp;", "").Replace("руб.", "").Replace("/комплект", "").Replace(".", ",");
            bool isRightPrice = float.TryParse(priceText, NumberStyles.Any, CultureInfo.CurrentCulture, out float price);
            if (isRightPrice) cardPrice = price;
            return cardPrice;
        }
        private static string GetPicture(string pictureNode, HtmlNode cardNode)
        {
            string? cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(cardPicture)) cardPicture = cardNode.SelectSingleNode("div/div[1]/div[1]/div[1]/a/img")?.Attributes.FirstOrDefault(x => x.Name == "data-src")?.Value;
            if (String.IsNullOrEmpty(cardPicture)) cardPicture = "SadClient.jpg";
            if (cardPicture == "https://turbok.by/img/no-photo--lg.png") cardPicture = "SadClient.jpg";
            return cardPicture;
        }
        private static string GetAkvilonPicture(string pictureNode, string prefixNode, HtmlNode cardNode)
        {
            string? cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == "data-src")?.Value;
            if (!String.IsNullOrEmpty(cardPicture))
            {
                cardPicture = string.Concat(prefixNode, cardPicture);
            }
            else
            {
                cardPicture = "SadClient.jpg";
            }
            return cardPicture;
        }
        private static string GetCatNumber(string catNumberNode, HtmlNode cardNode)
        {
            string? cardCatNumber = cardNode.SelectSingleNode(catNumberNode)?.InnerText.Trim();
            if (String.IsNullOrEmpty(cardCatNumber)) cardCatNumber = "-----";
            return cardCatNumber;
        }
        private static string GetStatus(string statusNode, HtmlNode cardNode)
        {
            string? cardStatus = cardNode.SelectSingleNode(statusNode)?.InnerText.Trim();
            if (String.IsNullOrEmpty(cardStatus)) cardStatus = "Неизвестный статус";
            return cardStatus;
        }
        private static string GetCardUrl(string cardUrlNode, HtmlNode cardNode)
        {
            string? cardUrl = cardNode.SelectSingleNode(cardUrlNode).Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;
            return cardUrl;
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
            var cardNumber = cardName.Remove(cardName.Length - 1)[(charIndexForTrim + 1)..].TrimEnd().Replace("&quot", "");
            if (string.IsNullOrEmpty(cardNumber)) cardNumber = "-----";
            cardName = cardName.Substring(0, charIndexForTrim).Trim().Replace("&quot", "");
            return cardNumber;
        }
    }
}
