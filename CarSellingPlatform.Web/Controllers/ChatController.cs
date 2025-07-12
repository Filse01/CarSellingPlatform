using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSellingPlatform.Web.Controllers;

public class ChatController : BaseController
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        string? userId = GetUserId();
        IEnumerable<IndexChatViewModel> chats = await _chatService.ListAllChat(userId);
        return View(chats);
    }

    public async Task<IActionResult> Create(Guid carId)
    {
        var userId = GetUserId();
        bool create = await _chatService.CreateAsync(userId, carId);
        if (create == false)
        {
            return RedirectToAction("Index");
        }
        return RedirectToAction("Index");
    }
}