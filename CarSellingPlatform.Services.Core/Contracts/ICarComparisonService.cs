using CarSellingPlatform.Data.Models.Car;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarComparisonService
{
    Task<IEnumerable<Car>> GetAllCars();
    
    Task<Car?> GetCar(string searchTerm);
}