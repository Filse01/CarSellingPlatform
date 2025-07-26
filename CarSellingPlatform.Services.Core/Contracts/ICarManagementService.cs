using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.CarManagement;
using CarSellingPlatform.Web.ViewModels.UserManager;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarManagementService
{
    Task<PagedListViewModel<CarManagementIndexView>> ListPagedAsync(string? userId, int pageNumber, int pageSize);
    Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId);
    Task<bool> EditCarAsync(string userId,EditCarViewModel model);
    Task<bool> SoftDeleteCarAsync(Guid id, string userId);
}