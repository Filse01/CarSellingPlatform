using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.ForumManagement;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IForumManagerService
{
    Task<PagedListViewModel<IndexForumManagement>> ListPagedAsync(string? userId, int pageNumber, int pageSize);
    Task<bool> HardDeleteDealership(Guid id, string userId);
}