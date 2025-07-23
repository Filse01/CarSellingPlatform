using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public abstract class BaseAdminController : Controller
{
    protected bool IsUserAuthenticated()
    {
        return this.User.Identity.IsAuthenticated;
    }
    protected string? GetUserId()
    {
        string? userId = null;
        bool isAuthenticated = this.IsUserAuthenticated();
        if (isAuthenticated)
        {
            userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        return userId;
    }
}