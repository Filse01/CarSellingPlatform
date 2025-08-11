using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;

public class DealershipManagementController : BaseAdminController
{
    private IDealershipManagementService _dealershipManagementService;

    public DealershipManagementController(IDealershipManagementService dealershipManagementService)
    {
        _dealershipManagementService = dealershipManagementService;
    }
    public async Task<IActionResult> Index(int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var pagedCars = await _dealershipManagementService.ListPagedAsync(userId, page, pageSize);
        return View(pagedCars);
    }
    public async Task<IActionResult> DeleteDealership(Guid id)
    {
        bool deleteResult = await _dealershipManagementService
            .HardDeleteDealership(id,GetUserId());
        TempData["SuccessMessage"] = deleteResult
            ? "Dealership deleted successfully."
            : "Failed to delete Dealership.";
        return RedirectToAction(nameof(Index));
    }
}