using HtmlAgilityPack;

namespace PriceSpy.Web.Models
{
    public class HtmlReader
    {
        private readonly HttpClient httpClient;

        public HtmlReader()
        {
            this.httpClient = new HttpClient();
        }

        //public SiteModel Read()
        //{
        //    var siteModel = new SiteModel();
        //    string html = File.ReadAllText("d:\\Documents\\search2.txt");
        //    HtmlDocument doc = new HtmlDocument();
        //    doc.LoadHtml(html);
        //    HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");

        //    foreach (var cardNode in nodes)
        //    {
        //        CardTemplate cardTemplate = new CardTemplate();

        //        cardTemplate.Name = cardNode.SelectSingleNode("div[2]/div[1]/div[1]").InnerText.Trim();
        //        cardTemplate.Price = cardNode.SelectSingleNode("div[2]/div[2]/div").InnerText.Trim();
        //        cardTemplate.PictureUrl = cardNode.SelectSingleNode("div[1]/a/div/img").Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
        //        cardTemplate.CatNumber = cardNode.SelectSingleNode("div[2]/div[1]/div[2]/p[1]").InnerText.Trim();
        //        cardTemplate.Status = cardNode.SelectSingleNode("div[2]/div[1]/p").InnerText.Trim();

        //        siteModel.CardTemplates.Add(cardTemplate);
        //    }

        //    return siteModel;
        //}

        public async Task<SiteModel> GetTurbokResultsAsync(string search, CancellationToken cancellationToken)
        {
            var httpResult = await httpClient.GetAsync($"https://turbok.by/search?gender=&gender=&catlist=0&searchText={search}", cancellationToken);

            if (!httpResult.IsSuccessStatusCode)
                throw new Exception("Something wrong");

            var htmlResult = await httpResult.Content.ReadAsStringAsync(cancellationToken);

            var siteModel = new SiteModel { Name = "Turbok"};

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlResult);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//*[@id=\"changeGrid\"]");

            if (nodes != null)
            {
                foreach (var cardNode in nodes)
                {
                    CardTemplate cardTemplate = new CardTemplate();

                    cardTemplate.Name = cardNode.SelectSingleNode("div[2]/div[1]/div[1]").InnerText.Trim();
                    cardTemplate.Price = cardNode.SelectSingleNode("div[2]/div[2]/div").InnerText.Trim();
                    cardTemplate.PictureUrl = cardNode.SelectSingleNode("div[1]/a/div/img").Attributes.FirstOrDefault(x => x.Name == "src")?.Value ?? string.Empty;
                    cardTemplate.CatNumber = cardNode.SelectSingleNode("div[2]/div[1]/div[2]/p[1]").InnerText.Trim();
                    cardTemplate.Status = cardNode.SelectSingleNode("div[2]/div[1]/p").InnerText.Trim();

                    siteModel.CardTemplates.Add(cardTemplate);
                }

            }
            return siteModel;
        }

        public SiteModel GetSomethingResult(string search)
        {
            return null;
        }
    }
}
