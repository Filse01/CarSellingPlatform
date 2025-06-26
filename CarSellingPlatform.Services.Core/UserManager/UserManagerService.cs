using CarSellingPlatform.Data;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.UserManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core.UserManager;

public class UserManagerService : IUserManager
{
    private readonly CarSellingPlatformDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UserManagerService(CarSellingPlatformDbContext context, UserManager<IdentityUser> userManager)
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
                Roles = roles
            });
        }
    
        return users;
    }
}