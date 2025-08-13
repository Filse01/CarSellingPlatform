using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class CarComparisonService : ICarComparisonService
{
    private readonly IRepository<Car, Guid> _carRepository;

    public CarComparisonService(IRepository<Car, Guid> carRepository)
    {
        _carRepository = carRepository;
    }
    public async Task<IEnumerable<Car>> GetAllCars()
    {
        var allCars = await _carRepository.GetAllAttached()
            .Include(c => c.Brand)
            .Include(c => c.Engine)
            .Include(c => c.Category)
            .Include(c => c.FuelType)
            .Include(c => c.Transmission)
            .ToListAsync();
        return allCars;
    }

    public async Task<Car?> GetCar(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return null;
        }
        var allCars = await _carRepository.GetAllAttached()
            .Include(c => c.Brand)
            .Include(c => c.Engine)
            .Include(c => c.Category)
            .Include(c => c.FuelType)
            .Include(c => c.Transmission)
            .ToListAsync();
        return allCars
            .FirstOrDefault(c => string.Equals($"{c.Year} {c.Brand.Name} {c.Model}", searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}