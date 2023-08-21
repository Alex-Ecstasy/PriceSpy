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

        public async Task <IActionResult> Index()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            await RateHandler.GetRateFromFileAsync();
            return View("Index");
        }

        public async Task<IActionResult> Results(string searchQuery, string rate, CancellationToken cancellationToken)
        {
            await RateHandler.CheckRateAsync(rate);
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

        public async Task<IActionResult> PricesAsync(string searchQuery, string rate)
        {
            
            await RateHandler.CheckRateAsync(rate);
            if (string.IsNullOrEmpty(searchQuery)) return View("Prices");
            XmlHandler.Search(searchQuery);
            return View("Prices");
        }

        public async Task<IActionResult> FetchAsync(string searchQuery, string rate)
        {
            if (string.IsNullOrEmpty(rate))
            {
                SampleViewModel.TextInfo = "Rate is empty";
                return PartialView("Fetch");
            }
            await RateHandler.CheckRateAsync(rate);

            if (string.IsNullOrEmpty(searchQuery))
            {
                SampleViewModel.TextInfo = "Search query is empty";
                return PartialView("Fetch");
            }
            SampleViewModel.TextInfo = null;
            if (SampleViewModel.Shippers.Count == 0) XmlHandler.Load();
            XmlHandler.Search(searchQuery);
            /// if GetRateFromBank() rate not change in form
            return PartialView("Fetch");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}