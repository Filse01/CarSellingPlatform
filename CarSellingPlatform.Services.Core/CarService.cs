using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Models;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class CarService : ICarService
{
    private readonly CarSellingPlatformDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public CarService(CarSellingPlatformDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IndexCarViewModel>> ListAllAsync()
    {
        IEnumerable<IndexCarViewModel> allCars = await this._dbContext
            .Cars
            .Include(c => c.Category)
            .Include(c => c.FuelType)
            .Include(c => c.Brand)
            .Include(c => c.Engine)
            .Include(c => c.Transmission)
            .AsNoTracking()
            .Select(c => new IndexCarViewModel()
            {
                Id = c.Id,
                BrandName = c.Brand.Name,
                CategoryName = c.Category.Name,
                CarModel = c.Model,
                Description = c.Description,
                HorsePower = c.Engine.Horsepower,
                Color = c.Color,
                TransmissionTypeName = c.Transmission.Type,
                Year = c.Year,
                Displacement = c.Engine.Displacement,
                Price = c.Price,
                FuelTypeName = c.FuelType.Type,
                ImageUrl = c.ImageUrl,
            }).ToListAsync();
        return allCars;
    }

    public async Task<bool> AddCarAsync(string userId, AddCarViewModel model)
    {
        bool opResult = false;
        
        var user = _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            Engine newEngine = new Engine()
            {
                Id = new Guid(),
                Cylinders = model.Cylinders,
                Displacement = model.Displacement,
                Horsepower = model.Horsepower,
                EngineCode = model.EngineCode,
            };
            await _dbContext.Engines.AddAsync(newEngine);
            Car newCar = new Car()
            {
                BrandId = model.BrandId,
                Model = model.CarModel,
                ImageUrl = model.ImageUrl,
                Description = model.Description,
                Price = model.Price,
                Color = model.Color,
                Year = model.Year,
                CategoryId = model.CategoryId,
                SellerId = userId,
                EngineId = newEngine.Id,
                TransmissionId = model.TransmissionId,
                FuelTypeId = model.FuelTypeId,
            };
            await _dbContext.Cars.AddAsync(newCar);
            await _dbContext.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}