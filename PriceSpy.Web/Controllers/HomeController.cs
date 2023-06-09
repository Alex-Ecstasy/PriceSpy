using Microsoft.AspNetCore.Mvc;
using PriceSpy.Web.Models;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace PriceSpy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HtmlReader htmlReader;
        // private readonly XmlHandler xmlHandler;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            htmlReader = new HtmlReader();
        }

        public async Task<IActionResult> Index(string searchQuery, string rate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return View();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SampleViewModel sampleViewModel = new SampleViewModel(searchQuery, rate);

            XmlHandler.Read(sampleViewModel, searchQuery);

            sampleViewModel.Sites.Add(await htmlReader.GetTurbokResultsAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetMagnitResultAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetAkvilonResultAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetBelagroResult(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetMazrezervResult(searchQuery, cancellationToken));

            return View("Results", sampleViewModel);
        }

        public IActionResult Prices(string searchQuery, string rate)
        {
            
            SampleViewModel allShippers = new SampleViewModel(searchQuery, rate);
            if (string.IsNullOrEmpty(searchQuery)) return View("Prices", allShippers);
            XmlHandler.Read(allShippers, searchQuery);
            return View("Prices", allShippers);
        }

        public IActionResult Fetch(string searchQuery, string rate)
        {
            if (string.IsNullOrEmpty(searchQuery)) return View("Prices");
            SampleViewModel allShippers = new SampleViewModel(searchQuery, rate);
            XmlHandler.Read(allShippers, searchQuery);
            return PartialView("Fetch", allShippers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}