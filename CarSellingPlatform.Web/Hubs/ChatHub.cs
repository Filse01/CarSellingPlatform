using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using CarSellingPlatform.Web.ViewModels;
using Microsoft.AspNetCore.SignalR;

namespace CarSellingPlatform.Web.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}