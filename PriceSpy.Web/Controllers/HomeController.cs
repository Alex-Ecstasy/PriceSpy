using Microsoft.AspNetCore.Mvc;
using PriceSpy.Web.Models;
using System.Diagnostics;

namespace PriceSpy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Random _random = new Random();
        public SampleViewModel MySample = new SampleViewModel();
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View();
            }

            return View("Results");
        }

        public IActionResult Privacy()
        {
            SampleViewModel.NumberOfResults = _random.Next(1, 10);

            return View();
        }
        public IActionResult HtmlReader()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}