using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Services.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class UserManagementController : BaseController
{
    private readonly IUserManagerService _userManagerServiceService;

    
    public UserManagementController(IUserManagerService userManagerServiceService)
    {
        _userManagerServiceService = userManagerServiceService;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string userId = GetUserId();
        var allUsers =await this._userManagerServiceService.GetAllUsersAsync(userId);
        return View(allUsers);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateUserRole(string userId, string selectedRole)
    {
        var success = await _userManagerServiceService.UpdateUserRoleAsync(userId, selectedRole);

        TempData["SuccessMessage"] = success
            ? "User role updated successfully."
            : "Failed to update user role.";

        return RedirectToAction(nameof(Index));
    }
}