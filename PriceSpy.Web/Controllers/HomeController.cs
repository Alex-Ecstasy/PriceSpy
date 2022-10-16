using Microsoft.AspNetCore.Mvc;
using PriceSpy.Web.Models;
using System.Diagnostics;

namespace PriceSpy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HtmlReader htmlReader;
        
        //public SampleViewModel MySample = new SampleViewModel();
        

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

            SampleViewModel sampleViewModel = new SampleViewModel();

            sampleViewModel.Sites.Add(turbokResult);

            return View("Results", sampleViewModel);
        }

        public IActionResult Privacy()
        {
            SampleViewModel sampleViewModel = new SampleViewModel();

            //sampleViewModel.CardTemplates = Enumerable.Repeat(new CardTemplate(), CardTemplate.Results).ToArray();
            return View(sampleViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}