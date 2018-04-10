using System.Web.Mvc;
using BusBoard.ConsoleApp;
using BusBoard.Web.Models;
using BusBoard.Web.ViewModels;
using System.Timers;

namespace BusBoard.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Timer aTimer;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BusInfo(PostcodeSelection selection)
        {
            // Add some properties to the BusInfo view model with the data you want to render on the page.
            // Write code here to populate the view model with info from the APIs.
            // Then modify the view (in Views/Home/BusInfo.cshtml) to render upcoming buses.

            var program = new Program();
            var busData = program.FetchBusData(selection.Postcode);

            var info = new BusInfo(busData);
            return View(info);
        }
    

        public ActionResult About()
        {
            ViewBag.Message = "Information about this site";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us!";

            return View();
        }
    }
}