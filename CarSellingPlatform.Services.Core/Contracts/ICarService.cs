using CarSellingPlatform.Web.ViewModels.Car;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarService
{
    Task<PagedListViewModel<IndexCarViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize);

    Task<bool> AddCarAsync(string userId,AddCarViewModel model);

    Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId);
    Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId);
    
    Task<bool> EditCarAsync(string userId,EditCarViewModel model);
    Task<DeleteCarViewModel> GetDeleteCarAsync(Guid? id, string userId);
    Task<bool> SoftDeleteCarAsync(DeleteCarViewModel model, string userId);
}