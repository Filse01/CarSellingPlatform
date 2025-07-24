using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CarSellingPlatform.Data;
using CarSellingPlatform.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using CarSellingPlatform.Data.Models.Chat;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Web.Hubs;
[Authorize]
public class ChatHub : Hub
{
    private readonly CarSellingPlatformDbContext _context;
    
    public ChatHub(CarSellingPlatformDbContext context)
    {
        _context = context;
    }

  public async Task SendMessage(string message, string chatId)
    {
        
        if (string.IsNullOrWhiteSpace(message) || !Guid.TryParse(chatId, out var chatGuid))
        {
            return;
        }

        var userId = Context.UserIdentifier;
        if (userId == null) return;

        
        var chatMessage = new Message
        {
            Text = message,
            CreatorId = userId,
            ChatId = chatGuid,
            CreatedAt = DateTime.UtcNow
        };

        
        _context.Messages.Add(chatMessage);
        await _context.SaveChangesAsync();

       
        var user = await _context.Users.FindAsync(userId);
        var userName = user?.FirstName + " " + user?.LastName ?? "Anonymous";

        
        await Clients.Group(chatId).SendAsync("ReceiveMessage", userName, message);
    }

  
    public async Task GetChatHistory(string chatId)
    {
        if (!Guid.TryParse(chatId, out var chatGuid)) return;

        
        var history = await _context.Messages
            .Where(m => m.ChatId == chatGuid)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new { user = m.Creator.FirstName + " " + m.Creator.LastName, message = m.Text }) 
            .ToListAsync();

        
        await Clients.Caller.SendAsync("LoadHistory", history);
    }

    
    public override async Task OnConnectedAsync()
    {
        var chatId = Context.GetHttpContext()?.Request.Query["chatId"];

        if (!string.IsNullOrEmpty(chatId))
        {
           
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
