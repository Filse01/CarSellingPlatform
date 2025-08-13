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
public class DealershipManagementTests
{
    private Mock<IRepository<Dealership, Guid>> _mockDealershipRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private IDealershipManagementService _dealershipService;

    [SetUp]
    public void SetUp()
    {
        _mockDealershipRepository = new Mock<IRepository<Dealership, Guid>>();
        
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _dealershipService = new DealershipManagementService(
            _mockDealershipRepository.Object,
            _mockUserManager.Object
        );
    }
    [Test]
    public async Task ListPagedAsync_WhenDealershipsExist_ShouldReturnCorrectlyPaginatedViewModel()
    {
        // Arrange
        var dealerships = new List<Dealership>
        {
            new Dealership { Id = Guid.NewGuid(), Name = "Auto World", Owner = new ApplicationUser { FirstName = "John", LastName = "Doe" } },
            new Dealership { Id = Guid.NewGuid(), Name = "Car Emporium", Owner = new ApplicationUser { FirstName = "Jane", LastName = "Smith" } },
            new Dealership { Id = Guid.NewGuid(), Name = "Best Wheels", Owner = new ApplicationUser { FirstName = "Peter", LastName = "Jones" } }
        };
        
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        const int pageNumber = 2;
        const int pageSize = 2;

        // Act
        var result = await _dealershipService.ListPagedAsync(null, pageNumber, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count()); 
        Assert.AreEqual(2, result.TotalPages);  
        Assert.AreEqual("Best Wheels", result.Items.First().Name);
        Assert.AreEqual("Peter Jones", result.Items.First().OwnerName);
    }
    [Test]
    public async Task ListPagedAsync_WhenNoDealershipsExist_ShouldReturnEmptyViewModel()
    {
        // Arrange
        var dealerships = new List<Dealership>(); 
        
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        const int pageNumber = 1;
        const int pageSize = 10;

        // Act
        var result = await _dealershipService.ListPagedAsync(null, pageNumber, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Items);
        Assert.AreEqual(0, result.TotalPages);
        Assert.AreEqual(pageNumber, result.PageNumber);
    }
    [Test]
    public async Task HardDeleteDealership_WhenUserAndDealershipExist_ShouldDeleteAndReturnTrue()
    {
        var userId = "existing-user-id";
        var dealershipId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };
        var dealership = new Dealership { Id = dealershipId };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockDealershipRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Dealership, bool>>>()))
            .ReturnsAsync(dealership);

        var result = await _dealershipService.HardDeleteDealership(dealershipId, userId);

        Assert.IsTrue(result);
        _mockDealershipRepository.Verify(r => r.HardDelete(dealership), Times.Once);
        _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    
    [Test]
    public async Task HardDeleteDealership_WhenDealershipDoesNotExist_ShouldNotDeleteAndReturnFalse()
    {
        var userId = "existing-user-id";
        var dealershipId = Guid.NewGuid();
        var user = new ApplicationUser { Id = userId };

        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockDealershipRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Dealership, bool>>>()))
            .ReturnsAsync((Dealership)null);

        var result = await _dealershipService.HardDeleteDealership(dealershipId, userId);

        Assert.IsFalse(result);
        _mockDealershipRepository.Verify(r => r.HardDelete(It.IsAny<Dealership>()), Times.Never);
        _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}