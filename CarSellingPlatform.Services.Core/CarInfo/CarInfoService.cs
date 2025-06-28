using CarSellingPlatform.Data;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core.CarInfo;

public class CarInfoService : ICarInfoService
{
    private readonly CarSellingPlatformDbContext _context;

    public CarInfoService(CarSellingPlatformDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<AddCarBrand>> GetBrandsAsync()
    {
        IEnumerable<AddCarBrand> brands = await this._context
            .Brands
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
        IEnumerable<AddCarCategory> categories =  await this._context
            .Categories
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
        IEnumerable<AddCarFuelType> fuelTypes = await this._context
            .FuelTypes
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
        IEnumerable<AddCarTransmission> transmissions = await this._context
            .Transmissions
            .AsNoTracking()
            .Select(t => new AddCarTransmission()
            {
                Id = t.Id,
                Type = t.Type
            }).ToListAsync();
        return transmissions;
    }
}