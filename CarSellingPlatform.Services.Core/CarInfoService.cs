using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class CarInfoService : ICarInfoService
{
    private readonly CarSellingPlatformDbContext _context;
    private readonly IRepository<Brand, Guid> _brandRepository;
    private readonly IRepository<Category, Guid> _categoryRepository;
    private readonly IRepository<FuelType, Guid> _fuelTypeRepository;
    private readonly IRepository<Transmission, Guid> _transmissionRepository;
    private readonly IRepository<Dealership, Guid> _dealershipRepository;
    public CarInfoService(CarSellingPlatformDbContext context, IRepository<Brand, Guid> brandRepository, IRepository<Category, Guid> categoryRepository, IRepository<FuelType, Guid> fuelTypeRepository, IRepository<Transmission, Guid> transmissionRepository, IRepository<Dealership, Guid> dealershipRepository)
    {
        _context = context;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _fuelTypeRepository = fuelTypeRepository;
        _transmissionRepository = transmissionRepository;
        _dealershipRepository = dealershipRepository;
    }
    public async Task<IEnumerable<AddCarBrand>> GetBrandsAsync()
    {
        IEnumerable<AddCarBrand> brands = await _brandRepository
            .GetAllAttached()
            .AsNoTracking()
            .Select(b => new AddCarBrand()
            {
                Id = b.Id,
                Name = b.Name,
            }).ToListAsync();
        return brands;
    }

    public async Task<IEnumerable<AddCarCategory>> GetCategoriesAsync()
    {
        IEnumerable<AddCarCategory> categories =  await _categoryRepository
            .GetAllAttached()
            .AsNoTracking()
            .Select(c => new AddCarCategory()
            {
                Id = c.Id,
                Name = c.Name,
            }).ToListAsync();
        return categories;
    }

    public async Task<IEnumerable<AddCarFuelType>> GetFuelTypesAsync()
    {
        IEnumerable<AddCarFuelType> fuelTypes = await _fuelTypeRepository
            .GetAllAttached()
            .AsNoTracking()
            .Select(f => new AddCarFuelType()
            {
                Id = f.Id,
                Type = f.Type
            })
            .ToListAsync();
        return fuelTypes;
    }

    public async Task<IEnumerable<AddCarTransmission>> GetTransmissionsAsync()
    {
        IEnumerable<AddCarTransmission> transmissions = await _transmissionRepository
            .GetAllAttached()
            .AsNoTracking()
            .Select(t => new AddCarTransmission()
            {
                Id = t.Id,
                Type = t.Type
            }).ToListAsync();
        return transmissions;
    }

    public async Task<IEnumerable<AddCarDealership>> GetDealersihpAsync(string userId)
    {
        IEnumerable<AddCarDealership> dealerships = await _dealershipRepository
            .GetAllAttached()
            .AsNoTracking()
            .Where(d => d.OwnerId == userId)
            .Select(t => new AddCarDealership()
            {
                Id = t.Id,
                Name = t.Name
            }).ToListAsync();
        return dealerships;
    }
}