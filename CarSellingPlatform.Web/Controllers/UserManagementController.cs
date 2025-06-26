using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Services.Core.UserManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class UserManagementController : BaseController
{
    private readonly IUserManager _userManagerService;

    
    public UserManagementController(IUserManager userManagerService)
    {
        _userManagerService = userManagerService;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string userId = GetUserId();
        var allUsers =await this._userManagerService.GetAllUsersAsync(userId);
        return View(allUsers);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateUserRole(string userId, string selectedRole)
    {
        var success = await _userManagerService.UpdateUserRoleAsync(userId, selectedRole);

        TempData["SuccessMessage"] = success
            ? "User role updated successfully."
            : "Failed to update user role.";

        return RedirectToAction(nameof(Index));
    }
}