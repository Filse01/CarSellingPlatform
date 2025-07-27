using Microsoft.AspNetCore.Authorization;

namespace CarSellingPlatform.Web.Controllers
{
    using System.Diagnostics;

    using ViewModels;

    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        public HomeController(ILogger<HomeController> logger)
        {

        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFoundError");
            }
            else if (statusCode == 401 || statusCode == 403)
            {
                return View("UnathorizedError");
            }
            else if (statusCode == 500)
            {
                return View("Error");
            }
            else
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
    }
}
