namespace PriceSpy.Web.Models
{
    public class SiteNodes
    {
        public static async Task GetSellersNodesAsync()
        {
            await DataFromLocalFiles.GetSellersNodesFromFileAsync();
        }
        public static void SetNodes ()
        {
            string search = "searchQuery";
            SampleViewModel.Sellers.Clear();
            var turbokNode = new SellersNodes("Turbok", "https://turbok.by")
            {
                SiteName = "Turbok",
                SiteHost = "https://turbok.by",
                SearchResultsNode = "//*[@id=\"changeGrid\"]",
                NameNode = "div[2]/div[1]/div[1]",
                PriceNode = "div[2]/div[2]/div",
                PictureNode = "div[1]/a/div/img",
                PictureAttribute = "src",
                CatNumberNode = "div[2]/div[1]/div[2]/p[1]",
                StatusNode = "div[2]/div[1]/p",
                CardUrlNode = "div[1]/a"
            };
            turbokNode.SearchUrl = $"{turbokNode.SiteHost}/search?gender=&gender=&catlist=0&searchText={search}";
            SampleViewModel.Sellers.Add(turbokNode);

            var minskmagnitNode = new SellersNodes("Minskmagnit", "https://minskmagnit.by/")
            {
                SiteName = "Minskmagnit",
                SiteHost = "https://minskmagnit.by/",
                SearchResultsNode = "/html/body/main/div/article/div/section/ul/li",
                NameNode = "div/div[2]/div[1]/div",
                PriceNode = "div/div[2]/div[2]/div",
                PictureNode = "div/div[1]/a/img",
                PictureAttribute = "src",
                CatNumberNode = "div/div[2]/div[1]/span",
                StatusNode = "div/div[2]/div[2]/div[2]/span[1]",
                CardUrlNode = "div/div[1]/a"
            };
            minskmagnitNode.SearchUrl = $"{minskmagnitNode.SiteHost}/site_search?search_term={search}";
            SampleViewModel.Sellers.Add(minskmagnitNode);

            var akvilonNode = new SellersNodes("Akvilon", "https://akvilonavto.by")
            {
                SiteName = "Akvilon",
                SiteHost = "https://akvilonavto.by",
                SearchResultsNode = "//div[@data-productid]",
                ProdId = "//*[@id=\"ProdId\"]",
                NameNode = "../div/div[1]/div[2]/div[1]",
                PriceNode = "../div/div[1]/div[3]/div/span/span[2]",
                PictureNode = "../div/div[1]/div[1]/div[1]/img",
                PictureAttribute = "data-src",
                CatNumberNode = "",
                StatusNode = "../div/div[2]/div[1]/div[1]/div/div/span/span",
                CardUrlNode = "../div/div[1]/div[2]/div[1]/a"
            };
            akvilonNode.SearchUrl = $"{akvilonNode.SiteHost}/catalog/?q={search}";
            SampleViewModel.Sellers.Add(akvilonNode);

            var belagroNode = new SellersNodes("Belagro", "https://1belagro.by")
            {
                SiteName = "Belagro",
                SiteHost = "https://1belagro.by",
                SearchResultsNode = "/html/body/div[1]/div[1]/div[3]/div/div/div/div/div",
                NameNode = "div/a",
                PriceNode = "div[3]/span[1]",
                PictureNode = "div[1]/img",
                PictureAttribute = "src",
                CatNumberNode = "div[2]/div/span",
                StatusNode = "div[3]/span[2]",
                CardUrlNode = "div[2]/a"
            };
            belagroNode.SearchUrl = $"{belagroNode.SiteHost}/search/?q={search}";
            SampleViewModel.Sellers.Add(belagroNode);

            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //string encodedSearch = HttpUtility.UrlEncode(search, GetEncoding("windows-1251"));
            var mazrezervNode = new SellersNodes("Mazrezerv", "https://www.mazrezerv.ru")
            {
                SiteName = "Mazrezerv",
                SiteHost = "https://www.mazrezerv.ru",
                SearchResultsNode = "//*[@id=\"print\"]/table/tr",
                ProdId = "//*[@id=\"ProdId\"]",
                NameNode = "td[3]",
                PriceNode = "td[7]",
                PictureNode = "td[2]/a",
                PictureAttribute = "href",
                CatNumberNode = "td[4]",
                StatusNode = "td[5]",
                CardUrlNode = "td[3]/a"
            };
            mazrezervNode.SearchUrl = $"{mazrezervNode.SiteHost}/price/?caption={search}&search=full";
            SampleViewModel.Sellers.Add(mazrezervNode);
            DataFromLocalFiles.WriteNodesInFile();
        }
    }
}
