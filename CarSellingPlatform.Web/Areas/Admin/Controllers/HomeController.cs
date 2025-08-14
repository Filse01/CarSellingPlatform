using System.Diagnostics;
using CarSellingPlatform.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;
[Area("Admin")]
public class HomeController : BaseAdminController
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult GetError()
    {
        return View("InternalServerError");
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
            return View("InternalServerError");
        }
        else
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}