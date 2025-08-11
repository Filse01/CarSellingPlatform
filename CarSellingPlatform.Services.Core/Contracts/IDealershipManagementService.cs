using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.DealershipManagement;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IDealershipManagementService
{
    Task<PagedListViewModel<IndexDealershipManagementService>> ListPagedAsync(string? userId, int pageNumber, int pageSize);
    Task<bool> HardDeleteDealership(Guid id, string userId);
}