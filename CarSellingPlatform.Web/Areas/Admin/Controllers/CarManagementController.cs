using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;

public class CarManagementController : BaseAdminController
{
    private readonly ICarManagementService _carManagementService;
    private readonly ICarInfoService _carInfoService;
    public CarManagementController(ICarManagementService carManagementService, ICarInfoService carInfoService)
    {
        _carManagementService = carManagementService;
        _carInfoService = carInfoService;
    }
    public async Task<IActionResult> Index(int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var pagedCars = await _carManagementService.ListPagedAsync(userId, page, pageSize);
        return View(pagedCars);
    }
    [HttpGet]
    public async Task<IActionResult> EditCar(Guid id)
    {
        string? userId = GetUserId();
            
        EditCarViewModel? car = await _carManagementService.GetEditCarAsync(id, userId);
        car.Categories = await this._carInfoService.GetCategoriesAsync();
        car.Brands = await this._carInfoService.GetBrandsAsync();
        car.FuelTypes = await this._carInfoService.GetFuelTypesAsync();
        car.Transmissions = await this._carInfoService.GetTransmissionsAsync();
        return View(car);
    }

    [HttpPost]
    public async Task<IActionResult> EditCar(EditCarViewModel model)
    {
        var userId = GetUserId();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool editResult = await _carManagementService.EditCarAsync(userId, model);
        TempData["SuccessMessage"] = editResult
            ? "Car edited successfully."
            : "Failed to edit car.";
        if (editResult == false)
        {
            return View(model);
        }
        return this.RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        bool deleteResult = await _carManagementService
            .SoftDeleteCarAsync(id,GetUserId());
        TempData["SuccessMessage"] = deleteResult
            ? "Car deleted successfully."
            : "Failed to delete car.";
        return RedirectToAction(nameof(Index));
    }
}