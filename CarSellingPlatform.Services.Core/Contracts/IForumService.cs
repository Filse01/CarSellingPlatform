using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.Forum;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IForumService
{
    Task<PagedListViewModel<IndexPostViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize, string sortBy = "latest");
    Task<bool> AddPostAsync(string userId, AddPostInputModel model);
    Task<bool> AddCommentAsync(string userId, AddCommentInputModel model, Guid postId);
    Task<DetailsPostViewModel> GetDetailsPostAsync(Guid? id, string userId);
}