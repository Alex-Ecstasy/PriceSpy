﻿using HtmlAgilityPack;
using System.Globalization;
using System.Text;
using static System.Text.Encoding;
using System.Web;
using Microsoft.Data.Sqlite;

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
        public async Task<Seller> GetResultsAsync(string search, SellersNodes sellersNodes, SqliteConnection connection, CancellationToken cancellationToken)
        {
            Seller seller = CreateSeller(search, sellersNodes);
            try
            {
                var httpResult = await httpClient.GetAsync(seller.SearchUrl, cancellationToken);
                if (!httpResult.IsSuccessStatusCode) return seller;
                else
                {
                    seller.IsAvailable = true;
                    var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);
                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlResult);
                    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(sellersNodes.SearchResultsNode);
                    if (nodes != null)
                    {
                        var uniqueCards = new HashSet<string>();
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            var cardNode = nodes[i];
                            if (!string.IsNullOrEmpty(sellersNodes.ProdId))
                            {
                                string id = cardNode.GetAttributeValue("id", "");
                                if (string.IsNullOrEmpty(id)) continue;
                                cardNode = doc.DocumentNode.SelectSingleNode(sellersNodes.ProdId.Replace("ProdId", id));
                            }
                            Card card = ParseCard(cardNode, sellersNodes);
                            if (CardIsDuplicate(card.CardUrl, uniqueCards)) continue;
                            DataBaseHandler.FindElementInDb(card, connection);
                            seller.CardList.Add(card);
                        }
                        seller.CardList = seller.CardList.OrderByDescending(x => x.IsAvailable).ToList();
                        nodes.Clear();
                        uniqueCards.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                ResponseContent responseContent = new();
                responseContent.Message = ex.ToString();
                responseContent.isAvailable = false;
            }
            return seller;
        }
        private static Seller CreateSeller(string search, SellersNodes sellersNodes)
        {
            Seller seller = new(sellersNodes.SiteName);
            if (sellersNodes.SiteName == "Mazrezerv")
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                search = HttpUtility.UrlEncode(search, GetEncoding("windows-1251"));
            }
            seller.SearchUrl = sellersNodes.SearchUrl.Replace("searchQuery", search);
            return seller;
        }
        private static Card ParseCard(HtmlNode cardNode, SellersNodes sellersNodes)
        {
            Card card = new();
            try
            {
                card.UrlPrefix = sellersNodes.SiteHost;
                card.CardUrl = GetCardUrl(sellersNodes.CardUrlNode, cardNode);
                string name = GetName(sellersNodes.NameNode, cardNode);
                card.Name = GetName(sellersNodes.NameNode, cardNode);
                card.Price = GetPrice(sellersNodes.PriceNode, cardNode, sellersNodes.SiteHost);
                card.Picture = GetPicture(sellersNodes.PictureNode, card.UrlPrefix, cardNode, sellersNodes.PictureAttribute);
                card.CatNumber = GetCatNumber(sellersNodes.CatNumberNode, cardNode, ref name);
                card.Name = RemoveCatNumber(name, card.CatNumber);
                card.Status = GetStatus(sellersNodes.StatusNode, cardNode);
                card.IsAvailable = GetAvailable(card.Status);
            }
            catch (Exception)
            {
                Console.WriteLine(card.Name + ": Parse error " + string.Concat(card.UrlPrefix, card.CardUrl));
                card.IsAvailable = false;
                card.Status = "Parse error";
            }
            return card;
        }
        private static string GetName(string nameNode, HtmlNode cardNode)
        {
            string? cardName = String.Empty;
            if (cardNode.SelectSingleNode(nameNode) == null) return cardName = "-----";
            cardName = cardNode.SelectSingleNode(nameNode)?.InnerText.Trim() ?? string.Empty;
            cardName = RemoveUselessFromString(cardName);
            if (cardName == string.Empty) cardName = "-----";
            return cardName;
        }
        private static float GetPrice(string priceNode, HtmlNode cardNode, string host)
        {
            float cardPrice = 0;
            if (cardNode.SelectSingleNode(priceNode) == null) return cardPrice = 0;
            var priceText = cardNode.SelectSingleNode(priceNode).InnerText.Trim();
            priceText = RemoveUselessFromString(priceText).Replace(".", ",");
            bool isRightPrice = float.TryParse(priceText, NumberStyles.Any, CultureInfo.CurrentCulture, out float price);
            if (isRightPrice) cardPrice = price;
            if (host.Contains(".ru")) cardPrice = price * SampleViewModel.Rate;
            return (float)Math.Round(cardPrice, 2);
        }
        private static string GetPicture(string pictureNode, string prefixNode, HtmlNode cardNode, string pictureAttribute)
        {
            string? cardPicture = "~/SadClient.jpg";
            if (cardNode.SelectSingleNode(pictureNode) == null) return cardPicture = "~/SadClient.jpg";
            cardPicture = cardNode.SelectSingleNode(pictureNode)?.Attributes.FirstOrDefault(x => x.Name == pictureAttribute)?.Value;
            if (String.IsNullOrEmpty(cardPicture)) return cardPicture = "~/SadClient.jpg";
            if (cardPicture == "https://turbok.by/img/no-photo--lg.png" || cardPicture.Contains("catalog/catalog-photo-3.svg")) return cardPicture = "~/SadClient.jpg";
            if (prefixNode == "https://turbok.by" || prefixNode == "https://minskmagnit.by/") return cardPicture;
            if (prefixNode == "https://1belagro.by") cardPicture = GetFullPictureBelagro(cardPicture);
            if (!String.IsNullOrEmpty(cardPicture))
            {
                cardPicture = string.Concat(prefixNode, cardPicture);
            }
            else
            {
                cardPicture = "~/SadClient.jpg";
            }
            return cardPicture;
        }
        private static string GetCatNumber(string catNumberNode, HtmlNode cardNode, ref string name)
        {
            string? cardCatNumber = String.Empty;
            if (String.IsNullOrEmpty(catNumberNode)) return cardCatNumber = Splite(ref name);
            if (cardNode.SelectSingleNode(catNumberNode) == null) return cardCatNumber = "-----";
            cardCatNumber = cardNode.SelectSingleNode(catNumberNode)?.InnerText.Trim();
            if (String.IsNullOrEmpty(cardCatNumber)) cardCatNumber = "-----";
            if (cardCatNumber.Length > 30) return cardCatNumber.Remove(30);
            return cardCatNumber;
        }
        private static string GetStatus(string statusNode, HtmlNode cardNode)
        {
            string? cardStatus = cardNode.SelectSingleNode(statusNode)?.InnerText.Trim();
            cardStatus = cardStatus switch
            {
                "минск" => cardNode.SelectSingleNode(statusNode)?.Attributes[0].Value.Trim(),
                "city store-none" => "Под заказ",
                "city" => "В наличии",
                "0" => "Нет в наличии",
                null => "Неизвестный статус",
                "" => "Неизвестный статус",
                _ => cardStatus,

            };
            if (cardNode.SelectSingleNode("td[5]") != null && cardStatus != "Нет в наличии")
                return $"В наличии {cardNode.SelectSingleNode("td[5]").InnerText} шт."; // Mazrezerv only
            return cardStatus;
        }
        private static bool GetAvailable(string statusNode) => statusNode.ToLower() switch
        {
            "нет в наличии" => false,
            "неизвестный статус" => false,
            "под заказ" => false,
            "0" => false,
            "товар в пути" => false,
            "в наличии" => true,
            "менее 10 шт" => true,
            "на складе: >10 шт." => true,
            "на складе: <10 шт." => true,
            _ => true
        };
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
            string cardNumber = "-----";
            if (cardName.Length > 100)
            {
                cardName = cardName.Substring(cardName.IndexOf('%') + 1).Trim();
                cardName = cardName.Substring(0, cardName.IndexOf("\n")).Trim();
            }
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
            if (charIndexForTrim + 1 < cardName.Length)
            {
                cardNumber = cardName.Remove(cardName.Length - 1)[(charIndexForTrim + 1)..].TrimEnd().Replace("&quot", "");
                if (string.IsNullOrEmpty(cardNumber)) cardNumber = "-----";
                cardName = cardName.Substring(0, charIndexForTrim).Trim().Replace("&quot", "");
            }
            else
            {
                cardName = "-----";
            }

            return cardNumber;
        }
        private static string GetFullPictureBelagro(string url)
        {
            string cardPicture = url;

            cardPicture = cardPicture.Replace("resize_cache/", "").Replace("56_56_1/", "");

            return cardPicture;

        }
        private static string RemoveCatNumber(string cardName, string cardNumber)
        {
            if (cardName.Contains(cardNumber)) cardName = cardName.Replace(cardNumber, "").Trim();

            return cardName;
        }
        private static string RemoveUselessFromString(string s)
        {
            foreach (string removeThis in HtmlReader._stringsForReplace)
                s = s.Replace(removeThis, "");
            return s;
        }
        private static bool CardIsDuplicate(string cardUrl, HashSet<string> uniqueCards)
        {
            return !uniqueCards.Add(cardUrl);
        }
        private static string[] _stringsForReplace = new string[] { "&nbsp;", "р.", "руб.", "/комплект", "от", "&#34;", "&quot;" };
    }
}
