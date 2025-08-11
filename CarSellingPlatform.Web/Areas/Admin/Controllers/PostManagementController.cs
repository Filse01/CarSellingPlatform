using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Areas.Admin.Controllers;
[Area("Admin")] 
public class PostManagementController : BaseController
{
    private IForumManagerService _forumManagerService;

    public PostManagementController(IForumManagerService forumManagerService)
    {
        _forumManagerService = forumManagerService;
    }
    public async Task<IActionResult> Index(int page =1)
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        var pagedPosts = await _forumManagerService.ListPagedAsync(userId, page, pageSize);
        return View(pagedPosts);
    }
    public async Task<IActionResult> DeletePost(Guid id)
    {
        bool deleteResult = await _forumManagerService
            .HardDeleteDealership(id,GetUserId());
        TempData["SuccessMessage"] = deleteResult
            ? "Post deleted successfully."
            : "Failed to delete Post.";
        return RedirectToAction(nameof(Index));
    }
}