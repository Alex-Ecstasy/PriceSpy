using HtmlAgilityPack;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using static System.Text.Encoding;
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        private readonly HttpClient httpClient;
        public HtmlReader()
        {
            this.httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }
        public async Task<Seller> GetTurbokResultsAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller ("Turbok", "https://turbok.by");
            ResponseContent responseContent = new();
            try
            {
                
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/search?gender=&gender=&catlist=0&searchText={search}", cancellationToken);
                
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
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
                }
            }

            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;

        }
        public async Task<Seller> GetMagnitResultAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Minskmagnit", "https://minskmagnit.by");
            ResponseContent responseContent = new();

            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/site_search?search_term={search}", cancellationToken);

                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("/html/body/main/div/article/div/section/ul/li");
                    if (nodes != null)
                    {
                        siteModel.IsAvailable = true;
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            card.Name = GetName("div/div[2]/div[1]/div", cardNode);
                            card.Price = GetPrice("div/div[2]/div[2]/div", cardNode);
                            card.Picture = GetPicture("div/div[1]/a/img", cardNode);
                            card.CatNumber = GetCatNumber("div/div[2]/div[1]/span", cardNode);
                            card.Status = GetStatus("div/div[2]/div[2]/div[2]/span[1]", cardNode);
                            card.IsAvailable = GetAvailable(card.Status);
                            card.CardUrl = GetCardUrl("div/div[1]/a", cardNode);
                            card.Name = card.Name.Replace(card.CatNumber, "");
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }


            return siteModel;
        }
        public async Task<Seller> GetAkvilonResultAsync(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Akvilon", "https://akvilonavto.by");
            ResponseContent responseContent = new();
            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/catalog/?q={search}", cancellationToken);
                
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"view-showcase\"]/div");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            string name = GetName("div/div[1]/div[2]/div[1]", cardNode);
                            string pictureAttribute = "data-src";
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
                }
            }

            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;
        }
        public async Task<Seller> GetBelagroResult(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Belagro", "https://1belagro.by");
            ResponseContent responseContent = new();
            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/search/?q={search}", cancellationToken);
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"search_catalog\"]/table/tbody/tr");
                    if (nodes != null)
                    {
                        foreach (var cardNode in nodes)
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            string name = GetName("td/a", cardNode);
                            string pictureAttribute = "href";
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
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;


        }
        public async Task<Seller> GetMazrezervResult(string search, CancellationToken cancellationToken)
        {
            var siteModel = new Seller("Mazrezerv", "https://www.mazrezerv.ru");
            ResponseContent responseContent = new();

            try
            {
                var httpResult = await httpClient.GetAsync($"{siteModel.Host}/price/?caption={search}&search=full", cancellationToken);
                if (!httpResult.IsSuccessStatusCode) return siteModel;
                else
                {
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlResult);
                    siteModel.IsAvailable = true;
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"print\"]/table/tr");
                    if (nodes != null)
                    {
                        
                        foreach (var cardNode in nodes.Skip(2))
                        {
                            Card card = new Card();
                            card.UrlPrefix = siteModel.Host;
                            card.Name = GetName("td[3]", cardNode);
                            card.Price = GetPrice("td[7]", cardNode) * SampleViewModel.Rate;
                            string pictureAttribute = "href";
                            card.Picture = GetPictureFromAttribute("td[2]/a", card.UrlPrefix, cardNode, pictureAttribute);
                            card.CatNumber = GetCatNumber("td[4]", cardNode);
                            card.Status = GetStatus("td[6]/b", cardNode);
                            if (card.Status == "нет в наличии") card.IsAvailable = false;
                            else
                            {
                                card.IsAvailable = true;
                                card.Status = $"В наличии {cardNode.SelectSingleNode("td[5]").InnerText} шт.";
                            }
                            card.CardUrl = GetCardUrl("td[3]/a", cardNode);
                            siteModel.CardList.Add(card);
                        }
                        siteModel.CardList = siteModel.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }

            return siteModel;
        }
        private static string GetName(string nameNode, HtmlNode cardNode)
        {
            string? cardName = String.Empty;
            if (cardNode.SelectSingleNode(nameNode) == null) return cardName = "-----";
            cardName = cardNode.SelectSingleNode(nameNode)?.InnerText.Trim().Replace("&#34;", "").Replace("&quot;", "") ?? string.Empty;
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
            string? cardPicture = "SadClient.jpg";
            if (cardNode.SelectSingleNode(pictureNode) == null) return cardPicture = "SadClient.jpg";
            cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(cardPicture)) cardPicture = cardNode.SelectSingleNode("div/div[1]/div[1]/div[1]/a/img")?.Attributes.FirstOrDefault(x => x.Name == "data-src")?.Value;
            if (String.IsNullOrEmpty(cardPicture)) cardPicture = "SadClient.jpg";
            if (cardPicture == "https://turbok.by/img/no-photo--lg.png") cardPicture = "SadClient.jpg";
            return cardPicture;
        }
        private static string GetPictureFromAttribute(string pictureNode, string prefixNode, HtmlNode cardNode, string pictureAttribute)
        {
            string? cardPicture = "SadClient.jpg";
            if (cardNode.SelectSingleNode(pictureNode) == null) return cardPicture = "SadClient.jpg";
            cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == pictureAttribute)?.Value;
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
            string? cardCatNumber = String.Empty;
            if (cardNode.SelectSingleNode(catNumberNode) == null) return cardCatNumber = "-----";
            cardCatNumber = cardNode.SelectSingleNode(catNumberNode)?.InnerText.Trim();
            if (String.IsNullOrEmpty(cardCatNumber)) cardCatNumber = "-----";
            return cardCatNumber;
        }
        private static string GetStatus(string statusNode, HtmlNode cardNode)
        {
            string? cardStatus = "Неизвестный статус";
            if (cardNode.SelectSingleNode(statusNode) == null) return cardStatus = "Неизвестный статус";
            cardStatus = cardNode.SelectSingleNode(statusNode)?.InnerText.Trim();
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
            string? cardStatus = "Неизвестный статус";
            if (cardNode.SelectSingleNode(statusNode) == null) return cardStatus = "Неизвестный статус";
            cardStatus = cardNode.SelectSingleNode(statusNode)?.Attributes[0].Value.Trim();
            if (cardStatus == "city store-none") cardStatus = "Под заказ";
            if (cardStatus == "city") cardStatus = "В наличии";
            if (String.IsNullOrEmpty(cardStatus)) cardStatus = "Неизвестный статус";
            return cardStatus;
        }
        private static string GetCardUrl(string cardUrlNode, HtmlNode cardNode)
        {
            string? cardUrl = String.Empty;
            if (cardNode.SelectSingleNode(cardUrlNode) == null) return cardUrl = String.Empty;
            cardUrl = cardNode.SelectSingleNode(cardUrlNode).Attributes.FirstOrDefault(x => x.Name == "href")?.Value ?? string.Empty;
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
