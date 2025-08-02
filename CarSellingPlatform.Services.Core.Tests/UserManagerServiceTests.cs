using CarSellingPlatform.Data.Interfaces.Repository;
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
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private IUserManagerService _userManagerService;

        [SetUp]
        public void SetUp()
        {
            _chatRepositoryMock = new Mock<IRepository<Chat, Guid>>();
            var store = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
            
            _userManagerService = new UserManagerService(_mockUserManager.Object, _chatRepositoryMock.Object);
        }
        

        [Test]
        public async Task ListPagedAsync_WhenUsersExist_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1", UserName = "Alice" },
                new ApplicationUser { Id = "user2", UserName = "Bob" },
                new ApplicationUser { Id = "user3", UserName = "Charlie" }
            };

            // Mock the .Users property to return our test list
            var mockUserQueryable = users.BuildMock();
            _mockUserManager.Setup(m => m.Users).Returns(mockUserQueryable);
            
            // Mock GetRolesAsync for each user
            _mockUserManager.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string> { "User" });
            
            var pageNumber = 1;
            var pageSize = 2;

            // Act
            var result = await _userManagerService.ListPagedAsync(null, pageNumber, pageSize);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Items.Count()); 
            Assert.AreEqual("Alice", result.Items.First().UserName);
        }

        [Test]
        public async Task ListPagedAsync_WhenNoUsersExist_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyUserList = new List<ApplicationUser>();
            var mockUserQueryable = emptyUserList.BuildMock();
            _mockUserManager.Setup(m => m.Users).Returns(mockUserQueryable);
            
            // Act
            var result = await _userManagerService.ListPagedAsync(null, 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result.Items);
            Assert.AreEqual(0, result.TotalPages);
        }
        [Test]
        public async Task UpdateUserRoleAsync_WithValidUserAndRole_ShouldSucceed()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId };
            var oldRoles = new List<string> { "User" };
            var newRole = "Admin";

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(oldRoles);
            
            // Mock successful role removal and addition
            _mockUserManager.Setup(um => um.RemoveFromRolesAsync(user, oldRoles))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(um => um.AddToRoleAsync(user, newRole))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userManagerService.UpdateUserRoleAsync(userId, newRole);

            // Assert
            Assert.IsTrue(result);
            _mockUserManager.Verify(um => um.RemoveFromRolesAsync(user, oldRoles), Times.Once);
            _mockUserManager.Verify(um => um.AddToRoleAsync(user, newRole), Times.Once);
        }

        [Test]
        public async Task UpdateUserRoleAsync_WhenRemovingOldRolesFails_ShouldReturnFalse()
        {
            // Arrange
            var userId = "test-user-id";
            var user = new ApplicationUser { Id = userId };
            var oldRoles = new List<string> { "User" };
            var newRole = "Admin";

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            _mockUserManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(oldRoles);
            
            // Mock FAILED role removal
            _mockUserManager.Setup(um => um.RemoveFromRolesAsync(user, oldRoles))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _userManagerService.UpdateUserRoleAsync(userId, newRole);

            // Assert
            Assert.IsFalse(result);
            // Verify that we never tried to add the new role because the process failed early
            _mockUserManager.Verify(um => um.AddToRoleAsync(user, newRole), Times.Never);
        }
        [Test]
        public async Task DeleteUser_WhenUserExists_ShouldDeleteUserAndChatsAndReturnTrue()
        {
            // Arrange
            var userId = "user-to-delete";
            var user = new ApplicationUser { Id = userId };
            var chats = new List<Chat>
            {
                new Chat { SellerId = userId }, // A chat where the user is the seller
                new Chat { UserId = userId }   // A chat where the user is the buyer
            };

            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
            
            var mockChatQueryable = chats.BuildMock();
            _chatRepositoryMock.Setup(r => r.GetAllAttached()).Returns(mockChatQueryable);
            
            _chatRepositoryMock.Setup(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()));
            _chatRepositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mockUserManager.Setup(um => um.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userManagerService.DeleteUser(userId);

            // Assert
            Assert.IsTrue(result);
            _chatRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()), Times.Exactly(2));
            _chatRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockUserManager.Verify(um => um.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task DeleteUser_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var userId = "non-existent-user";
            _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);
            
            // Act
            var result = await _userManagerService.DeleteUser(userId);

            // Assert
            Assert.IsFalse(result);
            _mockUserManager.Verify(um => um.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
            _chatRepositoryMock.Verify(r => r.HardDeleteRange(It.IsAny<IEnumerable<Chat>>()), Times.Never);
        }
}