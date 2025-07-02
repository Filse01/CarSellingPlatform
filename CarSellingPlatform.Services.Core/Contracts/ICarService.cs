using CarSellingPlatform.Web.ViewModels.Car;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarService
{
    Task<IEnumerable<IndexCarViewModel>> ListAllAsync();
    Task<bool> AddCarAsync(string userId,AddCarViewModel model);
}