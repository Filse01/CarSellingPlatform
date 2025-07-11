using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CarSellingPlatform.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Web.Hubs;
[Authorize]
public class ChatHub : Hub
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public async Task SendMessage(string message)
    {
        var user = Context.User.Identity.Name;
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}