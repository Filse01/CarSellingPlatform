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
}