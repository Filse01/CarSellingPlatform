using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.DealershipManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class DealershipManagementService : IDealershipManagementService
{
    private IRepository<Dealership, Guid> _dealershipRepository;
    private UserManager<ApplicationUser> _userManager;
    public DealershipManagementService(IRepository<Dealership, Guid> dealershipRepository, UserManager<ApplicationUser> userManager)
    {
        _dealershipRepository = dealershipRepository;
        _userManager = userManager;
    }
    public async Task<PagedListViewModel<IndexDealershipManagementService>> ListPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _dealershipRepository.GetAllAttached()
            .Include(d => d.Owner)
            .AsNoTracking();
        int totalCount = await query.CountAsync();
        var cars = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new IndexDealershipManagementService
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                PhoneNumber = c.PhoneNumber,
                OwnerName = c.Owner.FirstName + " " + c.Owner.LastName,
            })
            .ToListAsync();

        return new PagedListViewModel<IndexDealershipManagementService>
        {
            Items = cars,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<bool> HardDeleteDealership(Guid id, string userId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        Dealership deletedCar = await _dealershipRepository.SingleOrDefaultAsync(c => c.Id == id);
        if (user != null && deletedCar != null)
        {
            _dealershipRepository.HardDelete(deletedCar);
            await _dealershipRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}