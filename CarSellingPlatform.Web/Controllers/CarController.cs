using CarSellingPlatform.Data;
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
    private readonly IDealerShipService _dealerShipService;
    public CarController(ICarInfoService carInfoService, ICarService carService, IDealerShipService dealerShipService)
    {
        _carInfoService = carInfoService;
        _carService = carService;
        _dealerShipService = dealerShipService;
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var brands = await _carInfoService.GetBrandsAsync();
        var categories = await _carInfoService.GetCategoriesAsync();
        var transmissionTypes = await _carInfoService.GetTransmissionsAsync();
        var pagedCars = await _carService.ListPagedAsync(userId, page, pageSize);
        pagedCars.Brands = brands.Select(b => new AddCarBrand
        {
            Id = b.Id,
            Name = b.Name
        });
        pagedCars.Categories = categories.Select(c => new AddCarCategory() { Id = c.Id, Name = c.Name });
        pagedCars.Transmissions = transmissionTypes.Select(t => new AddCarTransmission() { Id = t.Id, Type = t.Type });
        return View(pagedCars);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Filter(string search = "",
        Guid? brandId = null,
        int page = 1,
        Guid? categoryId = null,
        Guid? transmissionTypeId = null,
        int? minPrice = null, 
        int? maxPrice = null,
        int? minHp = null,    
        int? maxHp = null)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var pagedCars = await _carService.ListPagedAsync(userId, page, pageSize, search, brandId,categoryId, transmissionTypeId ,minPrice, maxPrice, minHp, maxHp);
        
        return PartialView("_CarListPartial", pagedCars);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, IFormFile imageFile)
    {
        string? userId = GetUserId();
        DetailsCarViewModel? car = await _carService.GetDetailsCarAsync(id, userId);
        if (car == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        if (car.DealershipId.HasValue)
        {
            var dealership = await _dealerShipService.GetDetailsDealershipAsync(car.DealershipId.Value, userId);

            if (dealership != null)
            {
                car.Dealership = dealership; 
            }
        }
        string referrer = Request.Headers["Referer"].ToString();
        string backUrl = Url.Action("Index", "Car");
        if (!string.IsNullOrEmpty(referrer) && referrer.Contains("/Car/FavoriteCars"))
        {
            backUrl = referrer;
        }
        ViewBag.BackUrl = backUrl;
        return View(car);
    }
    [HttpGet]
    public async Task<IActionResult> AddCar()
    {
        var userId = GetUserId();
        AddCarViewModel model = new AddCarViewModel()
        {
            Categories = await this._carInfoService.GetCategoriesAsync(),
            Brands = await this._carInfoService.GetBrandsAsync(),
            FuelTypes = await this._carInfoService.GetFuelTypesAsync(),
            Transmissions = await this._carInfoService.GetTransmissionsAsync(),
            Dealerships = await this._carInfoService.GetDealersihpAsync(userId)
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddCar(AddCarViewModel model, IFormFile? imageFile)
    {
        var userId = GetUserId();
        if (!this.ModelState.IsValid)
        {
            return View(model);
        }
        bool addResult = await _carService.AddCarAsync(userId, model, imageFile);
        TempData["SuccessMessage"] = addResult
            ? "Car added successfully."
            : "Failed to add car.";
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
        string referrer = Request.Headers["Referer"].ToString();
        string backUrl = Url.Action("Index", "Car");
        if (!string.IsNullOrEmpty(referrer) && referrer.Contains("/Car/MyCars"))
        {
            backUrl = referrer;
        }
        ViewBag.BackUrl = backUrl;
        return View(car);
    }

    [HttpPost]
    public async Task<IActionResult> EditCar(EditCarViewModel model, IFormFile? imageFile)
    {
        var userId = GetUserId();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool editResult = await _carService.EditCarAsync(userId, model, imageFile);
        TempData["SuccessMessage"] = editResult
            ? "Car edited successfully."
            : "Failed to edit car.";
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
        TempData["SuccessMessage"] = deleteResult
            ? "Car deleted successfully."
            : "Failed to delete car.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> FavoriteCars(int? id, int page =1)
    {
        const int pageSize = 10;
        string userId = GetUserId();
        var pagedFavoriteCars = await _carService.GetFavoriteCarsAsync(userId, page, pageSize);
        return View(pagedFavoriteCars);
    }

    [HttpPost]
    public async Task<IActionResult> AddFavoriteCars(Guid id)
    {
        string? userId = GetUserId();
        if (id == null)
        {
            return RedirectToAction(nameof(Index));
        }
        bool opResult = await _carService.AddCarToFavoritesAsync(userId, id);
        TempData["SuccessMessage"] = opResult
            ? "Car added to favorites successfully."
            : "Failed to add car to favorites.";
        if (opResult == false)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(FavoriteCars));
    }

    [HttpPost]
    public async Task<IActionResult> RemoveFavoriteCars(Guid id)
    {
        string? userId = GetUserId();
        
        bool opResult = await _carService.RemoveCarFromFavoritesAsync(userId, id);
        TempData["SuccessMessage"] = opResult
            ? "Car removed from favorites successfully."
            : "Failed to remove car from favorites.";
        if (opResult == false)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(FavoriteCars));
    }

    [HttpGet]
    public async Task<IActionResult> MyCars(int? id, int page = 1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var pagedMyCars = await _carService.MyCarsPagedAsync(userId, page, pageSize);
        return View(pagedMyCars);
    }
    [AllowAnonymous]
    public async Task<IActionResult> GetCarImage(Guid id)
    {
        var imageResult = await _carService.GetCarImageByIdAsync(id);
        if (imageResult.HasValue)
        {
            return File(imageResult.Value.ImageData, imageResult.Value.ContentType);
        }

        return NotFound();
    }
}
