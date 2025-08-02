using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.CarManagement;
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
    public async Task<PagedListViewModel<UserManagementIndexViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _userManager.Users;

        int totalCount = await query.CountAsync();
        var usersViewModel = new List<UserManagementIndexViewModel>();
        var paginatedUsers = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        foreach (var user in paginatedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersViewModel.Add(new UserManagementIndexViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
                EmailConfirmed = user.EmailConfirmed,
            });
        }
        return new PagedListViewModel<UserManagementIndexViewModel>
        {
            Items = usersViewModel,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        var sellerChats = await _chatRepository.GetAllAttached()
            .Where(c => c.SellerId == user.Id)
            .ToListAsync();
        var userChats = await _chatRepository.GetAllAttached()
            .Where(c => c.UserId == user.Id)
            .ToListAsync();

        _chatRepository.HardDeleteRange(sellerChats); 
        _chatRepository.HardDeleteRange(userChats); 
        await _chatRepository.SaveChangesAsync(); 
        var result = await _userManager.DeleteAsync(user); 
        
        return result.Succeeded;
    }
}