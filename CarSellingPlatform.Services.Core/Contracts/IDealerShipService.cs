using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.Dealership;
using Microsoft.AspNetCore.Http;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IDealerShipService
{
    Task<PagedListViewModel<IndexCarViewModel>> ListPagedAsync(Guid id, string? userId, int pageNumber, int pageSize);
    Task<PagedListViewModel<IndexDealershipViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize);
    Task<bool> AddDealershipAsync(string userId, AddDealershipInputModel model, IFormFile? imageFile);
    Task<DetailsDealershipViewModel> GetDetailsDealershipAsync(Guid? id, string userId);
    Task<(byte[] ImageData, string ContentType)?> GetDealershipImageByIdAsync(Guid id);
    Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId);
}