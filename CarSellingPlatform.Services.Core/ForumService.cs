using AspNetCoreArchTemplate.Data.Migrations;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.Dealership;
using CarSellingPlatform.Web.ViewModels.Forum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class ForumService : IForumService
{
    private readonly IRepository<Post, Guid> _forumRepository;
    private readonly IRepository<Comment, Guid> _commentRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public ForumService(IRepository<Post, Guid> forumRepository,IRepository<Comment,Guid> commentRepository,UserManager<ApplicationUser> userManager)
    {
        _forumRepository = forumRepository;
        _commentRepository = commentRepository;
        _userManager = userManager;
    }

    public async Task<PagedListViewModel<IndexPostViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _forumRepository.GetAllAttached()
            .Include(x => x.Comments)
            .AsNoTracking();
        int totalCount = await query.CountAsync();
        var posts = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new IndexPostViewModel
            {
                Id = c.Id,
                Title = c.Title,
                Text = c.Text,
                Author = c.Author,
                Comments = c.Comments
            })
            .ToListAsync();

        return new PagedListViewModel<IndexPostViewModel>
        {
            Items = posts,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<bool> AddPostAsync(string userId, AddPostInputModel model)
    {
        bool opResult = false;
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            Post newPost = new Post()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Text = model.Text,
                AuthorId = userId,
            };
            await _forumRepository.AddAsync(newPost);
            await _forumRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }

    public async Task<bool> AddCommentAsync(string userId, AddCommentInputModel model, Guid postId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            Comment newComment = new Comment()
            {
                Id = Guid.NewGuid(),
                Text = model.NewCommentText,
                Author = user,
                AuthorId = userId,
                PostId = postId,
                CreatedAt = DateTime.Now,
            };
            await _commentRepository.AddAsync(newComment);
            await _commentRepository.SaveChangesAsync();
        }
        return opResult;
    }

    public async Task<DetailsPostViewModel> GetDetailsPostAsync(Guid? id, string userId)
    {
        DetailsPostViewModel? model = null;
        if (id.HasValue)
        {
            Post postModel = await _forumRepository
                .GetAllAttached()
                .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
                .Include(p => p.Author)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            if (postModel != null)
            {
                model = new DetailsPostViewModel()
                {
                    Post = postModel,
                    IsOwner = !string.IsNullOrEmpty(userId) && postModel.AuthorId == userId,
                    NewCommentText = string.Empty ,
                    Author = postModel.Author,
                };
            }
        }
        return model;
    }
}