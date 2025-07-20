using CarSellingPlatform.Web.ViewModels.UserManager;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IUserManagerService
{
    Task<IEnumerable<UserManagementIndexViewModel>> GetAllUsersAsync(string userId);
    
    Task<bool> UpdateUserRoleAsync(string userId, string selectedRole);
}