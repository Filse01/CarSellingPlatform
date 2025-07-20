using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Repository;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Chat;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class ChatService : IChatService
{
    private readonly IRepository<Chat, Guid> _chatRepository;
    private readonly IRepository<Car, Guid> _carRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    public ChatService(IRepository<Chat, Guid> chatRepository, UserManager<ApplicationUser> userManager, IRepository<Car, Guid> carRepository)
    {
        _chatRepository = chatRepository;
        _userManager = userManager;
        _carRepository = carRepository;
    }

    public async Task<IEnumerable<IndexChatViewModel>> ListAllChat(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        IEnumerable<IndexChatViewModel> chats = await _chatRepository.GetAllAttached()
            .Include(c => c.Car)
            .ThenInclude(c => c.Brand)
            .Where(c => c.UserId == user.Id || c.SellerId == user.Id)
            .Select(c => new IndexChatViewModel()
            {
                Id = c.Id,
                User = c.User.FirstName + " " + c.User.LastName,
                Seller = c.Seller.FirstName + " " + c.Seller.LastName,
                CarModel = c.Car.Model,
                CarBrand = c.Car.Brand.Name,
            }).ToArrayAsync();
        return chats;
    }

    public async Task<bool> CreateAsync(string userId, Guid carId)
    {
        var opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        var car = await _carRepository.SingleOrDefaultAsync(c => c.Id == carId);
        var chat = await _chatRepository.SingleOrDefaultAsync(c => c.CarId == carId && c.UserId == userId && c.SellerId == car.SellerId);
        if (chat == null)
        {
            if (user != null)
            {
                Chat newChat = new Chat()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SellerId = car.SellerId,
                    CarId = car.Id,
                };
                await _chatRepository.AddAsync(newChat);
                await _chatRepository.SaveChangesAsync();
                opResult = true;
            }
        }
        
        return opResult;
    }

    public async Task<bool> DeleteAsync(Guid chatId)
    {
        var opResult = false;
        var chat = await _chatRepository.SingleOrDefaultAsync(c => c.Id == chatId);
        if (chat != null)
        {
            _chatRepository.HardDelete(chat);
            await _chatRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }
}