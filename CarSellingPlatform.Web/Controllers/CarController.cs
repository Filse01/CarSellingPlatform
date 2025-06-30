using CarSellingPlatform.Services.Core;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class CarController : BaseController
{
    private readonly ICarInfoService _carInfoService;
    private readonly ICarService _carService;
    public CarController(ICarInfoService carInfoService, ICarService carService)
    {
        _carInfoService = carInfoService;
        _carService = carService;
    }
    
    // [AllowAnonymous]
    // Task<IActionResult> Index()
    // {
    //     string? userId = GetUserId();
    //     return View();
    // }
    [HttpGet]
    public async Task<IActionResult> AddCar()
    {
        AddCarViewModel model = new AddCarViewModel()
        {
            Categories = await this._carInfoService.GetCategoriesAsync(),
            Brands = await this._carInfoService.GetBrandsAsync(),
            FuelTypes = await this._carInfoService.GetFuelTypesAsync(),
            Transmissions = await this._carInfoService.GetTransmissionsAsync(),
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCar(AddCarViewModel model)
    {
        var userId = GetUserId();
        if (!this.ModelState.IsValid)
        {
            return View(model);
        }
        bool addResult = await _carService.AddCarAsync(userId, model);
        if (addResult == false)
        {
            return View(model);
        }
        return RedirectToPage("/Index");
    }
}
