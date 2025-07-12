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
[HttpPost]
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
    [HttpGet("Chat/Room/{chatId}")]
    public async Task<IActionResult> Room(Guid chatId)
    {
        // You would typically perform validation here to ensure the
        // current user is allowed to be in this chat.

        var viewModel = new ChatViewModel
        {
            ChatId = chatId
        };

        return View(viewModel);
    }
}