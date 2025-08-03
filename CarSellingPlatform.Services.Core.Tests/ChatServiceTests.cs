using System.Linq.Expressions;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class ChatServiceTests
{
    private Mock<IRepository<Chat, Guid>> _mockChatRepository;
    private Mock<IRepository<Car, Guid>> _mockCarRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private IChatService _chatService;

    private const string TestUserId = "test-user-id";

    [SetUp]
    public void SetUp()
    {
        _mockChatRepository = new Mock<IRepository<Chat, Guid>>();
        _mockCarRepository = new Mock<IRepository<Car, Guid>>();
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        // Assuming ChatService only needs these two dependencies for this method
        _chatService = new ChatService(
            _mockChatRepository.Object,
            _mockUserManager.Object,
            _mockCarRepository.Object
        );
    }
    [Test]
        public async Task ListAllChat_WhenUserHasChats_ShouldReturnViewModelCollection()
        {
            
            var user = new ApplicationUser { Id = TestUserId, FirstName = "Test", LastName = "User" };
            var seller = new ApplicationUser { Id = "seller-id", FirstName = "John", LastName = "Seller" };
            var buyer = new ApplicationUser { Id = "buyer-id", FirstName = "Jane", LastName = "Buyer" };
            var car = new Car { Model = "Camry", Brand = new Brand { Name = "Toyota" } };

            var chats = new List<Chat>
            {
                // Chat where our user is the buyer
                new Chat { Id = Guid.NewGuid(), UserId = user.Id, User = user, SellerId = seller.Id, Seller = seller, Car = car },
                // Chat where our user is the seller
                new Chat { Id = Guid.NewGuid(), UserId = buyer.Id, User = buyer, SellerId = user.Id, Seller = user, Car = car },
                // A chat that should be filtered out
                new Chat { Id = Guid.NewGuid(), UserId = buyer.Id, User = buyer, SellerId = seller.Id, Seller = seller, Car = car }
            };
            
            _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
            var mockQueryable = chats.BuildMock();
            _mockChatRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            
            var result = await _chatService.ListAllChat(TestUserId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            
            var firstChat = result.First(c => c.User == "Test User");
            Assert.AreEqual("John Seller", firstChat.Seller);
            Assert.AreEqual("Camry", firstChat.CarModel);
        }

    [Test]
    public async Task ListAllChat_WhenUserHasNoChats_ShouldReturnEmptyCollection()
    {
        var userWithNoChats = new ApplicationUser { Id = "user-with-no-chats", FirstName = "Nochat", LastName = "User" };
        var userId = userWithNoChats.Id;
        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(userWithNoChats);
        var otherChats = new List<Chat>
        {
        };
        var mockQueryable = otherChats.BuildMock();
        _mockChatRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        var result = await _chatService.ListAllChat(userId);
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
    [Test]
    public async Task CreateAsync_WhenChatDoesNotExist_ShouldCreateChatAndReturnTrue() 
    {
            
            var user = new ApplicationUser { Id = TestUserId };
            var car = new Car { Id = Guid.NewGuid(), SellerId = "seller-id" };
            _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId))
                .ReturnsAsync(new ApplicationUser { Id = TestUserId });
            
            _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(car);
            
        
            _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
                .ReturnsAsync((Chat)null);

            _mockChatRepository.Setup(r => r.AddAsync(It.IsAny<Chat>())).Returns(Task.CompletedTask);
            _mockChatRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            
            
            var result = await _chatService.CreateAsync(TestUserId, car.Id);

            
            Assert.IsTrue(result);
            _mockChatRepository.Verify(r => r.AddAsync(It.IsAny<Chat>()), Times.Once);
            _mockChatRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenChatAlreadyExists_ShouldNotCreateChatAndReturnFalse() 
    {
            
            var user = new ApplicationUser { Id = TestUserId };
            var car = new Car { Id = Guid.NewGuid(), SellerId = "seller-id" };
            var existingChat = new Chat { UserId = user.Id, CarId = car.Id, SellerId = car.SellerId };

            _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(car);
            
            
            _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
                .ReturnsAsync(existingChat);
            
            
            var result = await _chatService.CreateAsync(TestUserId, car.Id);

            
            Assert.IsFalse(result);
            _mockChatRepository.Verify(r => r.AddAsync(It.IsAny<Chat>()), Times.Never);
            _mockChatRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task DeleteAsync_WhenChatExists_ShouldDeleteChatAndReturnTrue()
    {
        
        var chatId = Guid.NewGuid();
        var existingChat = new Chat { Id = chatId };

        _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
            .ReturnsAsync(existingChat);
            
        _mockChatRepository.Setup(r => r.HardDelete(It.IsAny<Chat>()));
        _mockChatRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        
        var result = await _chatService.DeleteAsync(chatId);

        
        Assert.IsTrue(result);
        _mockChatRepository.Verify(r => r.HardDelete(existingChat), Times.Once);
        _mockChatRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenChatDoesNotExist_ShouldReturnFalse()
    {
        
        var chatId = Guid.NewGuid();
        
        _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
            .ReturnsAsync((Chat)null);
            
        
        var result = await _chatService.DeleteAsync(chatId);

        
        Assert.IsFalse(result);
        _mockChatRepository.Verify(r => r.HardDelete(It.IsAny<Chat>()), Times.Never);
        _mockChatRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetChatId_WhenChatExists_ShouldReturnCorrectChatId()
    {
        
        var userId = TestUserId;
        var carId = Guid.NewGuid();
        var expectedChatId = Guid.NewGuid();
        var existingChat = new Chat { Id = expectedChatId, UserId = userId, CarId = carId };

        _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
            .ReturnsAsync(existingChat);

        
        var result = await _chatService.GetChatId(userId, carId);

        
        Assert.AreEqual(expectedChatId, result);
    }

    [Test]
    public async Task GetChatId_WhenChatDoesNotExist_ShouldReturnGuidEmpty()
    {
        
        var userId = TestUserId;
        var carId = Guid.NewGuid();
        
        _mockChatRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Chat, bool>>>()))
            .ReturnsAsync((Chat)null);
            
        
        var result = await _chatService.GetChatId(userId, carId);

        
        Assert.AreEqual(Guid.Empty, result);
    }
}