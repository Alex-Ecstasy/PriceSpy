using Microsoft.AspNetCore.Mvc;
using PriceSpy.Web.Models;
using System.Diagnostics;

namespace PriceSpy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HtmlReader htmlReader;
        private readonly XmlHandler xmlHandler;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            htmlReader = new HtmlReader();
        }

        public async Task<IActionResult> Index(string searchQuery, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View();
            }

            var turbokResult = await htmlReader.GetTurbokResultsAsync(searchQuery, cancellationToken);
            var magnitResult = await htmlReader.GetMagnitResultAsync(searchQuery, cancellationToken);
            var akvilonResult = await htmlReader.GetAkvilonResultAsync(searchQuery, cancellationToken);
            SampleViewModel sampleViewModel = new SampleViewModel();
            SampleViewModel.Search = searchQuery;

            sampleViewModel.Sites.Add(turbokResult);
            sampleViewModel.Sites.Add(magnitResult);
            sampleViewModel.Sites.Add(akvilonResult);

            AllShippers allShippers = new AllShippers();
            XmlHandler.Read(allShippers);
            XmlHandler.Search(allShippers, searchQuery);
            return View("Results", sampleViewModel);
        }

        public IActionResult Privacy(string searchQuery)
        {
            AllShippers allShippers = new AllShippers();
            XmlHandler.Read(allShippers);
            XmlHandler.Search(allShippers, searchQuery);
            return View("Privacy", allShippers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}