using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.ForumManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class ForumManagerService : IForumManagerService
{
    private IRepository<Post, Guid> _postRepository;
    private UserManager<ApplicationUser> _userManager;
    public ForumManagerService(IRepository<Post, Guid> postRepository, UserManager<ApplicationUser> userManager)
    {
        _postRepository = postRepository;
        _userManager = userManager;
    }
    public async Task<PagedListViewModel<IndexForumManagement>> ListPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _postRepository.GetAllAttached()
            .Include(d => d.Author)
            .AsNoTracking();
        int totalCount = await query.CountAsync();
        var cars = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new IndexForumManagement()
            {
                Id = c.Id,
                Title = c.Title,
                AuthorName = c.Author.FirstName + " " + c.Author.LastName,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync();

        return new PagedListViewModel<IndexForumManagement>
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
        Post deletedCar = await _postRepository.SingleOrDefaultAsync(c => c.Id == id);
        if (user != null && deletedCar != null)
        {
            _postRepository.HardDelete(deletedCar);
            await _postRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}