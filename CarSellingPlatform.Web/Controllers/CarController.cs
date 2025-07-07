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
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        string? userId = GetUserId();
        var cars = await _carService.ListAllAsync(userId);
        return View(cars);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        string? userId = GetUserId();
        DetailsCarViewModel? car = await _carService.GetDetailsCarAsync(id, userId);
        if (car == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(car);
    }
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
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditCar(Guid id)
    {
        string? userId = GetUserId();
            
        EditCarViewModel? car = await _carService.GetEditCarAsync(id, userId);
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

        bool editResult = await _carService.EditCarAsync(userId, model);
        if (editResult == false)
        {
            return View(model);
        }
        return this.RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        string? userId = GetUserId();
        DeleteCarViewModel? car = await _carService.GetDeleteCarAsync(id, userId);
        if (car == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(car);
    }


    [HttpPost]
    public async Task<IActionResult> DeleteCar(DeleteCarViewModel model)
    {
        bool deleteResult = await _carService
            .SoftDeleteCarAsync(model,GetUserId());
        return RedirectToAction(nameof(Index));
    }
}
