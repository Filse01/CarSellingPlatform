using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Http;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarService
{
    Task<PagedListViewModel<IndexCarViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize, string? search = null, Guid? brandId = null);

    Task<bool> AddCarAsync(string userId,AddCarViewModel model, IFormFile? imageFile);

    Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId);
    Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId);
    
    Task<bool> EditCarAsync(string userId,EditCarViewModel model, IFormFile? imageFile);
    Task<DeleteCarViewModel> GetDeleteCarAsync(Guid? id, string userId);
    Task<bool> SoftDeleteCarAsync(DeleteCarViewModel model, string userId);
    Task<PagedListViewModel<FavoriteCarViewModel>?> GetFavoriteCarsAsync(string userId, int pageNumber, int pageSize);
    Task<bool> AddCarToFavoritesAsync(string userId, Guid carId);
    Task<bool> RemoveCarFromFavoritesAsync(string userId, Guid carId);
    Task<PagedListViewModel<MyCarsViewModel>> MyCarsPagedAsync(string? userId, int pageNumber, int pageSize);
    Task<(byte[] ImageData, string ContentType)?> GetCarImageByIdAsync(Guid id);
}