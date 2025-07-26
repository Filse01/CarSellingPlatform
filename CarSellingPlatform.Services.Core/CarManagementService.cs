using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.CarManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class CarManagementService : ICarManagementService
{
    private readonly IRepository<Car, Guid> _carRepository;
    private readonly IRepository<Engine, Guid> _engineRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public CarManagementService(IRepository<Car, Guid> carRepository, IRepository<Engine, Guid> engineRepository, UserManager<ApplicationUser> userManager)
    {
        _carRepository = carRepository;
        _engineRepository = engineRepository;
        _userManager = userManager;
    }
    public async Task<IEnumerable<CarManagementIndexView>> GetAllUCars()
    {
        var cars = _carRepository.GetAllAttached()
            .Select(c => new CarManagementIndexView
            {
                Id = c.Id,
                Brand = c.Brand.Name,
                Model = c.Model,
                SellerName = c.Seller.FirstName + " " + c.Seller.LastName,
                Year = c.Year,
                Email = c.Seller.Email
            }).ToList();
        return cars;
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
    public async Task<bool> SoftDeleteCarAsync(Guid id, string userId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        Car deletedCar = await _carRepository.SingleOrDefaultAsync(c => c.Id == id);
        if (user != null && deletedCar != null)
        {
            deletedCar.IsDeleted = true;
            await _carRepository.UpdateAsync(deletedCar);
            await _carRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
    
}