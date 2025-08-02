using CarSellingPlatform.Web.ViewModels.Chat;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface IChatService
{
    public Task<IEnumerable<IndexChatViewModel>> ListAllChat(string userId);
    public Task<bool> CreateAsync(string userId, Guid carId);
    
    public Task<bool> DeleteAsync(Guid chatId);
    
    public Task<Guid> GetChatId(string userId, Guid carId);
}