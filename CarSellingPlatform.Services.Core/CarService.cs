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

    public async Task<IEnumerable<IndexCarViewModel>> ListAllAsync(string? userId)
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
                IsUserSeller = userId != null ?
                    c.SellerId.ToLower() == userId.ToLower() : false
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

    public async Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId)
    {
        DetailsCarViewModel? model = null;
        if (id.HasValue)
        {
            Car carModel = await _carRepository
                .GetAllAttached()
                .Include(c => c.Category)
                .Include(c => c.FuelType)
                .Include(c => c.Brand)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            if (carModel != null)
            {
                model = new DetailsCarViewModel()
                {
                    Id = carModel.Id,
                    BrandName = carModel.Brand.Name,
                    CategoryName = carModel.Category.Name,
                    CarModel = carModel.Model,
                    Description = carModel.Description,
                    HorsePower = carModel.Engine.Horsepower,
                    Color = carModel.Color,
                    TransmissionTypeName = carModel.Transmission.Type,
                    Year = carModel.Year,
                    ImageUrl = carModel.ImageUrl,
                    Displacement = carModel.Engine.Displacement,
                    Price = carModel.Price,
                    FuelTypeName = carModel.FuelType.Type,
                    IsUserSeller = userId != null ? carModel.SellerId.ToLower() == userId.ToLower() : false
                };
            }
        }
        return model;
    }

    public async Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId)
    {
        EditCarViewModel model = null;
        Engine oldEngine = null;
        if (id.HasValue)
        {
            Car? editCar = await _carRepository.GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            Engine editEngine = await _engineRepository.GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == editCar.EngineId);
            if (editCar != null && editCar.SellerId.ToLower() == userId.ToLower())
            {
                oldEngine = new Engine()
                {
                    Id = editEngine.Id,
                    Displacement = editEngine.Displacement,
                    Cylinders = editEngine.Cylinders,
                    Horsepower = editEngine.Horsepower,
                    EngineCode = editEngine.EngineCode,
                };
                model = new EditCarViewModel()
                {
                    Id = editCar.Id,
                    BrandId = editCar.BrandId,
                    CarModel = editCar.Model,
                    Description = editCar.Description,
                    Price = editCar.Price,
                    Color = editCar.Color,
                    TransmissionId = editCar.TransmissionId,
                    FuelTypeId = editCar.FuelTypeId,
                    Year = editCar.Year,
                    Displacement = oldEngine.Displacement,
                    Horsepower = oldEngine.Horsepower,
                    EngineCode = oldEngine.EngineCode,
                    ImageUrl = editCar.ImageUrl,
                    SellerId = userId,
                    Cylinders = oldEngine.Cylinders,
                    CategoryId = editCar.CategoryId,
                    EngineId = oldEngine.Id
                };
            }
            
        }
        return model;
    }

    public async Task<bool> EditCarAsync(string userId,EditCarViewModel model)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);

        Car updatedCar = await _carRepository.GetAllAttached()
            .SingleOrDefaultAsync(c => c.Id == model.Id);
        Engine updatedEngine = await _engineRepository.GetAllAttached()
            .SingleOrDefaultAsync(c => c.Id == updatedCar.EngineId);
        if (updatedCar != null && user != null)
        {
            updatedCar.Model = model.CarModel;
            updatedCar.ImageUrl = model.ImageUrl;
            updatedCar.Description = model.Description;
            updatedCar.Price = model.Price;
            updatedCar.Color = model.Color;
            updatedCar.TransmissionId = model.TransmissionId;
            updatedCar.FuelTypeId = model.FuelTypeId;
            updatedCar.Year = model.Year;
            updatedCar.BrandId = model.BrandId;
            updatedCar.CategoryId = model.CategoryId;
            updatedEngine.Cylinders = model.Cylinders;
            updatedEngine.Horsepower = model.Horsepower;
            updatedEngine.EngineCode = model.EngineCode;
            await _carRepository.UpdateAsync(updatedCar);
            await _engineRepository.UpdateAsync(updatedEngine);
            await _carRepository.SaveChangesAsync();
            await _engineRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}