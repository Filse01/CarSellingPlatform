using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.Dealership;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class DealershipController : BaseController
{
    private readonly IDealerShipService _dealershipService;
    private readonly ICarService _carService;
    private readonly ICarInfoService _carInfoService;
    public DealershipController(IDealerShipService dealershipService, ICarService carService, ICarInfoService carInfoService)
    {
        _dealershipService = dealershipService;
        _carService = carService;
        _carInfoService = carInfoService;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Index(int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        
        var pagedDealerships = await _dealershipService.ListPagedAsync(userId, page, pageSize);
        return View(pagedDealerships);
    }
    [HttpGet]
    public async Task<IActionResult> AddDealership()
    {
        var dealership = new AddDealershipInputModel();
        return View(dealership);
    }
    [HttpPost]
    public async Task<IActionResult> AddDealership(AddDealershipInputModel model, IFormFile? imageFile)
    {
        var userId = GetUserId();
        if (!this.ModelState.IsValid)
        {
            return View(model);
        }
        bool addResult = await _dealershipService.AddDealershipAsync(userId, model, imageFile);
        TempData["SuccessMessage"] = addResult
            ? "Dealership added successfully."
            : "Failed to add Dealership.";
        if (addResult == false)
        {
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> DetailsDealership(Guid id)
    {
        string? userId = GetUserId();
        DetailsDealershipViewModel? dealership = await _dealershipService.GetDetailsDealershipAsync(id, userId);
        if (dealership == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(dealership);
    }
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> CarDetails(Guid id, IFormFile imageFile)
    {
        string? userId = GetUserId();
        DetailsCarViewModel? car = await _dealershipService.GetDetailsCarAsync(id, userId);
        if (car == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(car);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> AllCars(Guid id,int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var brands = await _carInfoService.GetBrandsAsync();
        var pagedCars = await _dealershipService.ListPagedAsync(id,userId, page, pageSize);
        pagedCars.Brands = brands.Select(b => new AddCarBrand
        {
            Id = b.Id,
            Name = b.Name
        });
        return View(pagedCars);
    }
    [AllowAnonymous]
    public async Task<IActionResult> GetDealershipImage(Guid id)
    {
        var imageResult = await _dealershipService.GetDealershipImageByIdAsync(id);
        if (imageResult.HasValue)
        {
            return File(imageResult.Value.ImageData, imageResult.Value.ContentType);
        }

        return NotFound();
    }
    [HttpGet]
    public async Task<IActionResult> EditDealership(Guid id)
    {
        string? userId = GetUserId();
            
        EditDealershipInputModel? dealership = await _dealershipService.GetEditDealershipAsync(id, userId);
        if (dealership == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(dealership);
    }
    [HttpPost]
    public async Task<IActionResult> EditDealership(EditDealershipInputModel model, IFormFile? imageFile)
    {
        var userId = GetUserId();
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool editResult = await _dealershipService.EditDealershipAsync(userId, model, imageFile);
        TempData["SuccessMessage"] = editResult
            ? "Dealership edited successfully."
            : "Failed to edit Dealership.";
        if (editResult == false)
        {
            return View(model);
        }
        return this.RedirectToAction(nameof(DetailsDealership), new { id = model.Id });
    }
}