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

    // Inject your application's DbContext
    public ChatHub(CarSellingPlatformDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Called by the client to send a message to a specific chat.
    /// </summary>
    /// <param name="message">The text of the message.</param>
    /// <param name="chatId">The GUID of the chat to send the message to.</param>
    public async Task SendMessage(string message, string chatId)
    {
        // Validate the input
        if (string.IsNullOrWhiteSpace(message) || !Guid.TryParse(chatId, out var chatGuid))
        {
            // You could optionally send an error back to the caller
            return;
        }

        // Get the current user's ID from the claims
        var userId = Context.UserIdentifier;
        if (userId == null) return; // Should not happen due to [Authorize]

        // Create the message entity to save to the database
        var chatMessage = new Message
        {
            Text = message, // Sanitize this if you're rendering it as HTML
            CreatorId = userId,
            ChatId = chatGuid,
            CreatedAt = DateTime.UtcNow
        };

        // Save the message
        _context.Messages.Add(chatMessage);
        await _context.SaveChangesAsync();

        // Retrieve the user's name to display in the chat
        var user = await _context.Users.FindAsync(userId);
        var userName = user?.UserName ?? "Anonymous";

        // Broadcast the message to all clients in the same chat group
        await Clients.Group(chatId).SendAsync("ReceiveMessage", userName, message);
    }

    /// <summary>
    /// Called by the client after connecting to retrieve the chat's message history.
    /// </summary>
    /// <param name="chatId">The GUID of the chat to load history for.</param>
    public async Task GetChatHistory(string chatId)
    {
        if (!Guid.TryParse(chatId, out var chatGuid)) return;

        // Query the database for the message history
        var history = await _context.Messages
            .Where(m => m.ChatId == chatGuid)
            .OrderBy(m => m.CreatedAt)
            .Select(m => new { user = m.Creator.UserName, message = m.Text }) // Project to an anonymous object
            .ToListAsync();

        // Send the history only to the client that requested it
        await Clients.Caller.SendAsync("LoadHistory", history);
    }

    /// <summary>
    /// Called automatically when a client connects to the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        // Get the chatId from the query string of the connection
        var chatId = Context.GetHttpContext()?.Request.Query["chatId"];

        if (!string.IsNullOrEmpty(chatId))
        {
            // Add the connection to a group named after the chatId.
            // This isolates message broadcasting to specific chats.
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called automatically when a client disconnects.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // You could add logic here to remove the user from the group,
        // but SignalR handles this automatically.
        // This is a good place for logging or presence tracking.
        await base.OnDisconnectedAsync(exception);
    }
}
