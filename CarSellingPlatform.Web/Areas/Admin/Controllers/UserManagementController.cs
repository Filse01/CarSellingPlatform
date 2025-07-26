using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Services.Core;
using CarSellingPlatform.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;
[Area("Admin")]
public class UserManagementController : BaseAdminController
{
    private readonly IUserManagerService _userManagerServiceService;
    private readonly IUserManagerService _userManagerService;
    public UserManagementController(IUserManagerService userManagerServiceService, IUserManagerService userManagerService)
    {
        _userManagerServiceService = userManagerServiceService;
        _userManagerService = userManagerService;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(int page=1)
    {
        const int pageSize = 10;
        string userId = GetUserId();
        var allUsers =await this._userManagerServiceService.ListPagedAsync(userId, page, pageSize);
        return View(allUsers);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUserRole(string userId, string selectedRole)
    {
        var success = await _userManagerServiceService.UpdateUserRoleAsync(userId, selectedRole);

        TempData["SuccessMessage"] = success
            ? "User role updated successfully."
            : "Failed to update user role.";

        return RedirectToAction(nameof(Index));
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        bool opResult = await _userManagerService.DeleteUser(id);
        if (opResult == false)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}