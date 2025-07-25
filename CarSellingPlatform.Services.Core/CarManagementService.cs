using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.CarManagement;

namespace CarSellingPlatform.Services.Core;

public class CarManagementService : ICarManagementService
{
    private readonly IRepository<Car, Guid> _carRepository;

    public CarManagementService(IRepository<Car, Guid> carRepository)
    {
        _carRepository = carRepository;
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
}