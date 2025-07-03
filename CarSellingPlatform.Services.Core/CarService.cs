using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models;
using CarSellingPlatform.Data.Repository;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarSellingPlatform.Services.Core;

public class CarService : ICarService
{
    private readonly IRepository<Car,Guid> _carRepository;
    private readonly IRepository<Engine, Guid> _engineRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public CarService(IRepository<Car, Guid> carRepository, IRepository<Engine, Guid> engineRepository, UserManager<IdentityUser> userManager)
    {
        _carRepository = carRepository;
        _engineRepository = engineRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<IndexCarViewModel>> ListAllAsync()
    {
        IEnumerable<IndexCarViewModel> allCars = await _carRepository.GetAllAttached()
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
                Id = Guid.NewGuid(),
                Cylinders = model.Cylinders,
                Displacement = model.Displacement,
                Horsepower = model.Horsepower,
                EngineCode = model.EngineCode,
            };
            await _engineRepository.AddAsync(newEngine);
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
            await _carRepository.AddAsync(newCar);
            await _carRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}