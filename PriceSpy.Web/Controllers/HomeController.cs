using Microsoft.AspNetCore.Mvc;
using PriceSpy.Web.Models;
using System.Diagnostics;
using System.Text;

namespace PriceSpy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HtmlReader htmlReader;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            htmlReader = new HtmlReader();
        }

        public IActionResult Index()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            SampleViewModel.Rate = DataFromLocalFiles.GetExchangeRates();
            XmlHandler.Load();
            return View("Index");
        }

        public async Task<IActionResult> Results(string searchQuery, string rate, CancellationToken cancellationToken)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            SampleViewModel.Rate = SampleViewModel.GetRate(rate);
            if (string.IsNullOrWhiteSpace(searchQuery)) return View();
            SampleViewModel.Search = searchQuery;
            XmlHandler.Search(searchQuery);
            SampleViewModel.Sites.Clear();
            SampleViewModel.Sites.Add(await htmlReader.GetTurbokResultsAsync(searchQuery, cancellationToken));
            SampleViewModel.Sites.Add(await htmlReader.GetMagnitResultAsync(searchQuery, cancellationToken));
            SampleViewModel.Sites.Add(await htmlReader.GetAkvilonResultAsync(searchQuery, cancellationToken));
            SampleViewModel.Sites.Add(await htmlReader.GetBelagroResult(searchQuery, cancellationToken));
            SampleViewModel.Sites.Add(await htmlReader.GetMazrezervResult(searchQuery, cancellationToken));

            return View("Results");
        }

        public IActionResult Prices(string searchQuery, string rate)
        {
            SampleViewModel.Rate = SampleViewModel.GetRate(rate);
            if (string.IsNullOrEmpty(searchQuery)) return View("Prices");
            XmlHandler.Search(searchQuery);
            return View("Prices");
        }

        public IActionResult Fetch(string searchQuery, string rate)
        {
            SampleViewModel.Rate = SampleViewModel.GetRate(rate);
            if (string.IsNullOrEmpty(searchQuery))
            {
                SampleViewModel.TextInfo = "Search query is empty";
                return PartialView("Fetch");
            }
            SampleViewModel.TextInfo = null;
            XmlHandler.Search(searchQuery);
            return PartialView("Fetch");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}