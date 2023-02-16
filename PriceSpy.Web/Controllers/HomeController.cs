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
        private readonly XmlHandler xmlHandler;
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            htmlReader = new HtmlReader();
        }

        public async Task<IActionResult> Index(string searchQuery, string rate, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return View();
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            SampleViewModel sampleViewModel = new SampleViewModel();
            rate = rate.Replace(".", ",");
            if (!float.TryParse(rate, out float rateExchange)) rateExchange = 1;
            SampleViewModel.Rate = rateExchange;
            XmlHandler.Read(sampleViewModel);

            //var turbokResult = await htmlReader.GetTurbokResultsAsync(searchQuery, cancellationToken)
            //var magnitResult = await htmlReader.GetMagnitResultAsync(searchQuery, cancellationToken);
            //var akvilonResult = await htmlReader.GetAkvilonResultAsync(searchQuery, cancellationToken);
            //var belagroResult = await htmlReader.GetBelagroResult(searchQuery, cancellationToken);
            //var mazrezervResult = await htmlReader.GetMazrezervResult(searchQuery, cancellationToken);

            SampleViewModel.Rate = rateExchange;
            SampleViewModel.Search = searchQuery;

            sampleViewModel.Sites.Add(await htmlReader.GetTurbokResultsAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetMagnitResultAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetAkvilonResultAsync(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetBelagroResult(searchQuery, cancellationToken));
            sampleViewModel.Sites.Add(await htmlReader.GetMazrezervResult(searchQuery, cancellationToken));

            XmlHandler.Search(sampleViewModel, searchQuery);
            return View("Results", sampleViewModel);
        }

        public IActionResult Privacy(string searchQuery)
        {
            SampleViewModel allShippers = new SampleViewModel();
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