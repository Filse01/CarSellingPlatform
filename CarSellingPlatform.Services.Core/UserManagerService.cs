using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.UserManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class UserManagerService : IUserManager
{
    private readonly CarSellingPlatformDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagerService(CarSellingPlatformDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IEnumerable<UserManagementIndexViewModel>> GetAllUsersAsync(string userId)
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var users = new List<UserManagementIndexViewModel>();
    
        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            users.Add(new UserManagementIndexViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
                EmailConfirmed = user.EmailConfirmed,
            });
        }
    
        return users;
    }

    public async Task<bool> UpdateUserRoleAsync(string userId, string selectedRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return false;

        // Get current roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove all current roles
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false;
        }

        // Add the selected role
        var addResult = await _userManager.AddToRoleAsync(user, selectedRole);
        return addResult.Succeeded;
    }
}