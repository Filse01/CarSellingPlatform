using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class ChatController : BaseController
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}