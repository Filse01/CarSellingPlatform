using CarSellingPlatform.Web.ViewModels.Car;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarService
{
    Task<IEnumerable<IndexCarViewModel>> ListAllAsync(string? userId);
    Task<bool> AddCarAsync(string userId,AddCarViewModel model);

    Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId);
    Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId);
    
    Task<bool> EditCarAsync(string userId,EditCarViewModel model);
}