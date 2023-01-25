using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static System.Text.Encoding;
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
                //throw new Exception("Turbok wrong");
                return null;
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
                    card.IsAvailable = GetAvailable(card.Status);
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
                //throw new Exception("Magnit wrong");
                return null;
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
                    card.IsAvailable = GetAvailable(card.Status);
                    card.CardUrl = GetCardUrl("div/div[2]/div[1]/a", cardNode);
                    card.Name = card.Name.Replace(card.CatNumber, "");
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
                //throw new Exception("Akvilon wrong");
                return null;
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
                    string pictureAttribute = "data-src";
                    card.UrlPrefix = "https://akvilonavto.by";
                    card.Price = GetPrice("div/div[1]/div[3]/div/span/span[2]", cardNode);
                    card.Picture = GetPictureFromAttribute("div/div[1]/div[1]/div[1]/a/img", card.UrlPrefix, cardNode, pictureAttribute);
                    card.CatNumber = Splite(ref name);
                    card.Name = name;
                    card.Status = GetStatus("div/div[2]/div[1]/div[1]/div/div/span/span", cardNode);
                    card.IsAvailable = GetAvailable(card.Status);
                    card.CardUrl = GetCardUrl("div/div[1]/div[1]/div[1]/a", cardNode);
                    siteModel.CardList.Add(card);
                }
                siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }
        public async Task<Seller> GetBelagroResult(string search, CancellationToken cancellationToken)
        {

            var httpResult = await httpClient.GetAsync($"https://1belagro.by/search/?q={search}", cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
                //throw new Exception("Belagro wrong");
                return null;
            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            var siteModel = new Seller { Name = "Belagro" };
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"search_catalog\"]/table/tbody/tr");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    Card card = new Card();
                    string name = GetName("td/a", cardNode);
                    string pictureAttribute = "href";
                    card.UrlPrefix = "https://1belagro.by";
                    card.Price = GetPrice("td[2]/div[2]", cardNode);
                    card.Picture = GetPictureFromAttribute("td[1]/div/a", card.UrlPrefix, cardNode, pictureAttribute);
                    card.CatNumber = SpliteBelagro(ref name);
                    card.Name = name;
                    card.Status = GetBelagroStatus("td[1]/div/div/span", cardNode);
                    card.IsAvailable = GetAvailable(card.Status);
                    card.CardUrl = GetCardUrl("td[1]/a", cardNode);
                    siteModel.CardList.Add(card);
                }
                siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
            }
            return siteModel;
        }
        public async Task<Seller> GetMazrezervResult(string search, CancellationToken cancellationToken)
        {
            httpClient.DefaultRequestHeaders.Clear();
            var httpResult = await httpClient.GetAsync($"https://www.mazrezerv.ru/price/?caption={search}&search=full", cancellationToken);
            if (!httpResult.IsSuccessStatusCode)
                //throw new Exception("Mazrezerv wrong");
                return null;
            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
            //var res = await httpResult.Content();
            //string htmlResult = null;
            //var buffer = await httpResult.Content.ReadAsByteArrayAsync();
            //byte[] bytes = buffer.ToArray();
            //string htmlResult = encoding.GetString(bytes, 0, bytes.Length);
            //using (var sr = new StreamReader(await httpResult.Content.ReadAsStreamAsync(), Encoding.UTF8))
            //{
            //    htmlResult = sr.ReadToEnd();
            //}
            var siteModel = new Seller { Name = "Mazrezerv" };

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"print\"]/table/tr");
            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    //Card card = new Card();
                    //card.UrlPrefix = "https://www.mazrezerv.ru";
                    //card.Name = GetName("div[2]/div[1]/div[1]", cardNode);
                    //card.Price = GetPrice("div[2]/div[2]/div", cardNode);
                    //card.Picture = GetPicture("div[1]/a/div/img", cardNode);
                    //card.CatNumber = GetCatNumber("div[2]/div[1]/div[2]/p[1]", cardNode);
                    //card.Status = GetStatus("div[2]/div[1]/p", cardNode);
                    //if (card.Status == "В наличии") card.IsAvailable = true;
                    //card.CardUrl = GetCardUrl("div[1]/a", cardNode);
                    //siteModel.CardList.Add(card);
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
            if (cardNode.SelectSingleNode(priceNode) == null) return cardPrice = 0;
            var priceText = cardNode.SelectSingleNode(priceNode).InnerText.Trim().Replace("&nbsp;", "").Replace("р.", "").Replace("руб.", "").Replace("/комплект", "").Replace(".", ",");
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
        private static string GetPictureFromAttribute(string pictureNode, string prefixNode, HtmlNode cardNode, string pictureAttribute)
        {
            string? cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == pictureAttribute)?.Value;
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
        private static bool GetAvailable(string statusNode) => statusNode switch
        {
            "Нет в наличии" => false,
            "Неизвестный статус" => false,
            "Под заказ" => false,
            "В наличии" => true,
            "Менее 10 шт" => true,
            _ => false
        };
        private static string GetBelagroStatus(string statusNode, HtmlNode cardNode)
        {
            string? cardStatus = cardNode.SelectSingleNode(statusNode)?.Attributes[0].Value.Trim();
            if (cardStatus == "city store-none") cardStatus = "Под заказ";
            if (cardStatus == "city") cardStatus = "В наличии";
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
        private static string SpliteBelagro(ref string cardName)
        {

            int charIndexForTrim = cardName.IndexOf(' ');
            if (charIndexForTrim <= 0) charIndexForTrim = 0;
            var cardNumber = cardName.Substring(0, charIndexForTrim).Trim();
            if (string.IsNullOrEmpty(cardNumber)) cardNumber = "-----";
            cardName = cardName.Substring(charIndexForTrim).Trim();

            return cardNumber;
        }
    }
}
