using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.UserManager;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IUserManagerService
{
    Task<PagedListViewModel<UserManagementIndexViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize);
    
    Task<bool> UpdateUserRoleAsync(string userId, string selectedRole);
    Task<bool> DeleteUser(string userId);
}