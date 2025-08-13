using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.CarComparison;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;
[AllowAnonymous]
public class CarComparisonController : BaseController
{
    private ICarComparisonService _carComparisonService;

    public CarComparisonController(ICarComparisonService carComparisonService)
    {
        _carComparisonService = carComparisonService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var viewModel = new CarComparisonModel()
        {
            AvailableCars = await _carComparisonService.GetAllCars()
        };
        
        return View(viewModel);
    }
    [HttpPost]
    public async Task<IActionResult> Index(string searchTerm1, string searchTerm2)
    {
        var viewModel = new CarComparisonModel()
        {
            AvailableCars = await _carComparisonService.GetAllCars(),
            Car1 = await _carComparisonService.GetCar(searchTerm1),
            Car2 = await _carComparisonService.GetCar(searchTerm2),
            SearchTerm1 = searchTerm1,
            SearchTerm2 = searchTerm2
        };

        return View(viewModel);
    }
}