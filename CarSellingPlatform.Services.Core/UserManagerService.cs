using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.UserManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class UserManagerService : IUserManagerService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Chat, Guid> _chatRepository;
    public UserManagerService(UserManager<ApplicationUser> userManager, IRepository<Chat, Guid> chatRepository)
    {
        _userManager = userManager;
        _chatRepository = chatRepository;
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
        {
            return false;
        }
        var currentRoles = await _userManager.GetRolesAsync(user);
        
        if (currentRoles.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false;
        }
        
        var addResult = await _userManager.AddToRoleAsync(user, selectedRole);
        return addResult.Succeeded;
    }
    public async Task<bool> DeleteUser(string userId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        var sellerChats = await _chatRepository.GetAllAttached()
            .Where(c => c.SellerId == user.Id)
            .ToListAsync();
        var userChats = await _chatRepository.GetAllAttached()
            .Where(c => c.UserId == user.Id)
            .ToListAsync();
        if (user != null)
        {
            _chatRepository.HardDeleteRange(sellerChats);
            _chatRepository.HardDeleteRange(userChats);
            await _chatRepository.SaveChangesAsync();
            await _userManager.DeleteAsync(user);
            opResult = true;
        }
        return opResult;
    }
}