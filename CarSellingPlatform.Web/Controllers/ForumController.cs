using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Forum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class ForumController : BaseController
{
    private readonly IForumService _forumService;

    public ForumController(IForumService forumService)
    {
        _forumService = forumService;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Index(int page =1,string sortBy = "latest")
    {
        const int pageSize = 10;
        string? userId = GetUserId();
        
        var pagedDealerships = await _forumService.ListPagedAsync(userId, page, pageSize, sortBy);
        return View(pagedDealerships);
    }
    [HttpGet]
    public async Task<IActionResult> AddPost()
    {
        var post = new AddPostInputModel();
        return View(post);
    }

    [HttpPost]
    public async Task<IActionResult> AddPost(AddPostInputModel post)
    {
        var userId = GetUserId();
        if (!this.ModelState.IsValid)
        {
            return View(post);
        }
        bool addResult = await _forumService.AddPostAsync(userId, post);
        if (addResult == false)
        {
            return View(post);
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        string? userId = GetUserId();
        DetailsPostViewModel? post = await _forumService.GetDetailsPostAsync(id, userId);
        if (post == null)
        {
            return this.RedirectToAction(nameof(Index));
        }
        return View(post);
    }

    public async Task<IActionResult> AddComment(Guid id, AddCommentInputModel comment)
    {
        var userId = GetUserId();
        if (!this.ModelState.IsValid)
        {
            return RedirectToAction("Details", new { id = id });
        }
        bool addResult = await _forumService.AddCommentAsync(userId, comment, id);
        if (addResult == false)
        {
            return RedirectToAction("Details", new { id = id });
        }
        return RedirectToAction("Details", new { id = id });
    }
    
}