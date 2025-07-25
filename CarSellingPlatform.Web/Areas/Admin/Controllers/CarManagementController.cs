using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;

public class CarManagementController : BaseAdminController
{
    private readonly ICarManagementService _carManagementService;

    public CarManagementController(ICarManagementService carManagementService)
    {
        _carManagementService = carManagementService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var allCars = await _carManagementService.GetAllUCars();
        return View(allCars);
    }
}