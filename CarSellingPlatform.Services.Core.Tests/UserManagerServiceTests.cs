using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class UserManagerServiceTests
{
        private Mock<IRepository<Chat, Guid>> _chatRepositoryMock;
        private Mock<IRepository<Car, Guid>> _carRepositoryMock;
        private Mock<IRepository<UserCar, Guid>> _userCarRepositoryMock;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private IUserManagerService _userManagerService;

        [SetUp]
        public void SetUp()
        {
            _chatRepositoryMock = new Mock<IRepository<Chat, Guid>>();
            _carRepositoryMock = new Mock<IRepository<Car, Guid>>();
            _userCarRepositoryMock = new Mock<IRepository<UserCar, Guid>>();
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            
            _userManagerService = new UserManagerService(_mockUserManager.Object, _chatRepositoryMock.Object, _carRepositoryMock.Object, _userCarRepositoryMock.Object);
        }
        

        [Test]
        public async Task ListPagedAsync_WhenUsersExist_ShouldReturnAllUsers()
        {
            
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", UserName = "Alice" },
                new ApplicationUser { Id = "user2", UserName = "Bob" },
                new ApplicationUser { Id = "user3", UserName = "Charlie" }
            };

            
            var mockUserQueryable = users.BuildMock();
            _mockUserManager.Setup(m => m.Users).Returns(mockUserQueryable);
            
            
            _mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });
            
            var pageNumber = 1;
            var pageSize = 2;

            
            var result = await _userManagerService.ListPagedAsync(null, pageNumber, pageSize);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Items.Count()); 
            Assert.AreEqual("Alice", result.Items.First().UserName);
        }

        [Test]
        public async Task ListPagedAsync_WhenNoUsersExist_ShouldReturnEmptyList()
        {
            
            var emptyUserList = new List<ApplicationUser>();
            var mockUserQueryable = emptyUserList.BuildMock();
            _mockUserManager.Setup(m => m.Users).Returns(mockUserQueryable);
            
            
            var result = await _userManagerService.ListPagedAsync(null, 1, 10);

            
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Items);
            Assert.AreEqual(0, result.TotalPages);
        }
        [Test]
        public async Task UpdateUserRoleAsync_WithValidUserAndRole_ShouldSucceed()
        {
            
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId };
            var oldRoles = new List<string> { "User" };
            var newRole = "Admin";

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(oldRoles);
            
            
            _mockUserManager.Setup(um => um.RemoveFromRolesAsync(user, oldRoles))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, newRole))
                .ReturnsAsync(IdentityResult.Success);

            
            var result = await _userManagerService.UpdateUserRoleAsync(userId, newRole);

            
            Assert.IsTrue(result);
            _mockUserManager.Verify(um => um.RemoveFromRolesAsync(user, oldRoles), Times.Once);
            _mockUserManager.Verify(um => um.AddToRoleAsync(user, newRole), Times.Once);
        }

        [Test]
        public async Task UpdateUserRoleAsync_WhenRemovingOldRolesFails_ShouldReturnFalse()
        {
            
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId };
            var oldRoles = new List<string> { "User" };
            var newRole = "Admin";

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(oldRoles);
            
            // Mock FAILED role removal
            _mockUserManager.Setup(um => um.RemoveFromRolesAsync(user, oldRoles))
                .ReturnsAsync(IdentityResult.Failed());

            
            var result = await _userManagerService.UpdateUserRoleAsync(userId, newRole);

            
            Assert.IsFalse(result);
            
            _mockUserManager.Verify(um => um.AddToRoleAsync(user, newRole), Times.Never);
        }
        [Test]
        public async Task DeleteUser_WhenUserExists_ShouldDeleteUserAndChatsAndReturnTrue()
        {
            
            var userId = "user-to-delete";
            var user = new ApplicationUser { Id = userId };
            var chats = new List<Chat>
            {
                new Chat { SellerId = userId },
                new Chat { UserId = userId }  
            };
            var cars = new List<Car>
            {
                new Car { SellerId = userId }
            };
            var userCar = new List<UserCar>
            {
                new UserCar { UserId = userId, CarId = cars[0].Id }
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var mockChatQueryable = chats.BuildMock();
            _chatRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockChatQueryable);
            
            _chatRepositoryMock.Setup(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()));
            _chatRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);
            
            var mockCarQueryable = cars.BuildMock();
            _carRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockCarQueryable);
            _carRepositoryMock.Setup(r => r.HardDeleteRange(It.IsAny<IEnumerable<Car>>()));
            
            var mockUserCarRepository = userCar.BuildMock();
            _userCarRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockUserCarRepository);
            
            var result = await _userManagerService.DeleteUser(userId);

            
            Assert.IsTrue(result);
            _chatRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()), Times.Exactly(2));
            _chatRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockUserManager.Verify(um => um.DeleteAsync(user), Times.Once);
            _carRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<Car>>()), Times.Once); 
            _userCarRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<UserCar>>()), Times.Once); 
        }

        [Test]
        public async Task DeleteUser_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            
            var userId = "non-existent-user";
            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);
            
            
            var result = await _userManagerService.DeleteUser(userId);

            
            Assert.IsFalse(result);
            _mockUserManager.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
            _chatRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()), Times.Never);
        }
}