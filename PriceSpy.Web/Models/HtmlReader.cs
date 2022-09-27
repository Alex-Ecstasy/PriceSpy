using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        public string Read()
        {
            string html = File.ReadAllText("d:\\Documents\\html.txt");
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");

            foreach (var cardNode in nodes)
            {
                string cardName = cardNode.SelectSingleNode("div[2]/div[1]/div[1]/a/p").InnerText;
            }
            return null;
        }

        

    }
}
