using System.Linq.Expressions;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Dealership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class DealershipServiceTests
{
    private Mock<IRepository<Dealership, Guid>> _mockDealershipRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private Mock<IRepository<Car, Guid>> _mockCarRepository;
    private IDealerShipService _dealerShipService;

    [SetUp]
    public void SetUp()
    {
        _mockDealershipRepository = new Mock<IRepository<Dealership, Guid>>();

        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _mockCarRepository = new Mock<IRepository<Car, Guid>>();

        _dealerShipService = new DealerShipService(
            _mockDealershipRepository.Object,
            _mockUserManager.Object,
            _mockCarRepository.Object
        );
    }
    [Test]
    public async Task ListPagedAsync_WhenCarsExistForDealership_ShouldReturnFilteredAndPaginatedCars()
    {
        var dealershipId = Guid.NewGuid();
        var otherDealershipId = Guid.NewGuid();

        var cars = new List<Car>
        {
            // Cars for the target dealership
            new Car { Id = Guid.NewGuid(), Model = "A4", DealershipId = dealershipId, Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() },
            new Car { Id = Guid.NewGuid(), Model = "A6", DealershipId = dealershipId, Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() },
            // Car that should be filtered out
            new Car { Id = Guid.NewGuid(), Model = "Civic", DealershipId = otherDealershipId, Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() }
        };

        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.ListPagedAsync(dealershipId, null, 1, 5);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count());
        Assert.IsTrue(result.Items.All(c => c.BrandName != "Civic"));
        Assert.AreEqual(1, result.TotalPages);
    }

    [Test]
    public async Task ListPagedAsync_WhenUserIdIsProvided_ShouldSetUserSpecificFlagsCorrectly()
    {
        var dealershipId = Guid.NewGuid();
        var userId = "test-user-id";
        var otherUserId = "other-user-id";

        var cars = new List<Car>
        {
            // Car where the user is the seller
            new Car { Id = Guid.NewGuid(), Model = "SellerCar", SellerId = userId, DealershipId = dealershipId, UserCars = new List<UserCar>(), Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() },
            // Car where the user has favorited it
            new Car { Id = Guid.NewGuid(), Model = "FavoriteCar", SellerId = otherUserId, DealershipId = dealershipId, UserCars = new List<UserCar> { new UserCar { UserId = userId } }, Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() },
            // Car where the user has no relation
            new Car { Id = Guid.NewGuid(), Model = "NeutralCar", SellerId = otherUserId, DealershipId = dealershipId, UserCars = new List<UserCar>(), Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Engine = new Engine(), Transmission = new Transmission() }
        };
        
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.ListPagedAsync(dealershipId, userId, 1, 5);
        
        Assert.IsTrue(result.Items.First(c => c.CarModel == "SellerCar").IsUserSeller);
        Assert.IsFalse(result.Items.First(c => c.CarModel == "SellerCar").IsUserFavorite);

        Assert.IsFalse(result.Items.First(c => c.CarModel == "FavoriteCar").IsUserSeller);
        Assert.IsTrue(result.Items.First(c => c.CarModel == "FavoriteCar").IsUserFavorite);
        
        Assert.IsFalse(result.Items.First(c => c.CarModel == "NeutralCar").IsUserSeller);
        Assert.IsFalse(result.Items.First(c => c.CarModel == "NeutralCar").IsUserFavorite);
    }
    [Test]
    public async Task ListPagedAsync_WhenDealershipsExist_ShouldReturnPaginatedData()
    {
        var dealerships = new List<Dealership>
        {
            new Dealership { Id = Guid.NewGuid(), Name = "Dealership A", Address = "123 Main St" },
            new Dealership { Id = Guid.NewGuid(), Name = "Dealership B", Address = "456 Oak Ave" },
            new Dealership { Id = Guid.NewGuid(), Name = "Dealership C", Address = "789 Pine Ln" }
        };
    
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        const int pageNumber = 2;
        const int pageSize = 2;

        var result = await _dealerShipService.ListPagedAsync(null, pageNumber, pageSize);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count()); 
        Assert.AreEqual(2, result.TotalPages); 
        Assert.AreEqual("Dealership C", result.Items.First().Name);
    }
    
    [Test]
    public async Task ListPagedAsync_WhenNoDealershipsExist_ShouldReturnEmptyResult()
    {
        var dealerships = new List<Dealership>();
    
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.ListPagedAsync(null, 1, 10);

        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Items);
        Assert.AreEqual(0, result.TotalPages);
    }
    [Test]
    public async Task AddDealershipAsync_WhenUserExistsAndImageIsProvided_ShouldAddDealershipWithLogo()
    {
        var userId = "existing-user-id";
        var user = new ApplicationUser { Id = userId };
        var model = new AddDealershipInputModel { Name = "Test Dealership" };
    
        // Mock IFormFile
        var mockImageFile = new Mock<IFormFile>();
        var content = "Fake image content";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;
        mockImageFile.Setup(f => f.OpenReadStream()).Returns(ms);
        mockImageFile.Setup(f => f.FileName).Returns(fileName);
        mockImageFile.Setup(f => f.Length).Returns(ms.Length);
        mockImageFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, token) => ms.CopyTo(stream));

        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);

        var result = await _dealerShipService.AddDealershipAsync(userId, model, mockImageFile.Object);

        Assert.IsTrue(result);
        _mockDealershipRepository.Verify(r => r.AddAsync(It.Is<Dealership>(d => d.Name == model.Name && d.Logo != null)), Times.Once);
        _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddDealershipAsync_WhenUserDoesNotExist_ShouldNotAddDealershipAndReturnFalse()
    {
        var userId = "non-existent-user-id";
        var model = new AddDealershipInputModel { Name = "Test Dealership" };
    
        _mockUserManager.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync((ApplicationUser)null);

        var result = await _dealerShipService.AddDealershipAsync(userId, model, null);

        Assert.IsFalse(result);
        _mockDealershipRepository.Verify(r => r.AddAsync(It.IsAny<Dealership>()), Times.Never);
        _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetDetailsDealershipAsync_WhenDealershipExists_ShouldReturnViewModel()
    {
        var dealershipId = Guid.NewGuid();
        var dealership = new Dealership
        {
            Id = dealershipId,
            Name = "Prestige Motors",
            Address = "101 Luxury Lane",
            PhoneNumber = "555-0101"
        };
    
        // Create a list containing the test object
        var dealerships = new List<Dealership> { dealership };
    
        // Use BuildMock() to create an IQueryable that supports async operations
        var mockQueryable = dealerships.BuildMock();

        // Setup GetAllAttached() to return this mock queryable
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetDetailsDealershipAsync(dealershipId, "any-user-id");

        Assert.IsNotNull(result);
        Assert.AreEqual(dealershipId, result.Id);
        Assert.AreEqual("Prestige Motors", result.Name);
        Assert.AreEqual("101 Luxury Lane", result.Address);
    }

    [Test]
    public async Task GetDetailsDealershipAsync_WhenDealershipDoesNotExist_ShouldReturnNull()
    {
        var nonExistentId = Guid.NewGuid();

        // Create an empty list
        var dealerships = new List<Dealership>();

        // Build the mock queryable from the empty list
        var mockQueryable = dealerships.BuildMock();

        // Setup GetAllAttached() to return the empty mock queryable
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetDetailsDealershipAsync(nonExistentId, "any-user-id");

        Assert.IsNull(result);
    }
    [Test]
    public async Task GetDealershipImageByIdAsync_WhenDealershipHasLogo_ShouldReturnImageData()
    {
        var dealershipId = Guid.NewGuid();
        var imageData = new byte[] { 1, 2, 3, 4, 5 };
        var dealership = new Dealership { Id = dealershipId, Logo = imageData };

        _mockDealershipRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Dealership, bool>>>()))
            .ReturnsAsync(dealership);

        var result = await _dealerShipService.GetDealershipImageByIdAsync(dealershipId);

        Assert.IsNotNull(result);
        Assert.AreEqual(imageData, result.Value.ImageData);
        Assert.AreEqual("image/jpeg", result.Value.ContentType);
    }

    [Test]
    public async Task GetDealershipImageByIdAsync_WhenDealershipHasNoLogo_ShouldReturnNull()
    {
        var dealershipId = Guid.NewGuid();
        var dealershipWithNoLogo = new Dealership { Id = dealershipId, Logo = null };

        _mockDealershipRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Dealership, bool>>>()))
            .ReturnsAsync(dealershipWithNoLogo);

        var result = await _dealerShipService.GetDealershipImageByIdAsync(dealershipId);

        Assert.IsNull(result);
    }
    [Test]
    public async Task GetDetailsCarAsync_WhenCarExists_ShouldReturnViewModel()
    {
        var carId = Guid.NewGuid();
        var userId = "test-seller-id";
        var car = new Car
        {
            Id = carId,
            Model = "M5",
            Description = "A fast sedan.",
            Color = "Blue",
            Year = 2024,
            Price = 90000,
            SellerId = userId,
            Category = new Category { Name = "Sedan" },
            FuelType = new FuelType { Type = "Gasoline" },
            Brand = new Brand { Name = "BMW" },
            Engine = new Engine { Horsepower = 617, Displacement = "4.4L", Cylinders = 8 },
            Transmission = new Transmission { Type = "Automatic" },
            Dealership = new Dealership(),
            Seller = new ApplicationUser { PhoneNumber = "555-1234" }
        };
    
        var cars = new List<Car> { car };
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetDetailsCarAsync(carId, userId);

        Assert.IsNotNull(result);
        Assert.AreEqual("M5", result.CarModel);
        Assert.AreEqual("BMW", result.BrandName);
        Assert.AreEqual(617, result.HorsePower);
        Assert.AreEqual("555-1234", result.PhoneNubmer);
        Assert.IsTrue(result.IsUserSeller);
    }

    [Test]
    public async Task GetDetailsCarAsync_WhenCarDoesNotExist_ShouldReturnNull()
    {
        var nonExistentCarId = Guid.NewGuid();
        var cars = new List<Car>();
    
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetDetailsCarAsync(nonExistentCarId, "any-user");

        Assert.IsNull(result);
    }
    [Test]
    public async Task GetEditDealershipAsync_WhenUserIsOwner_ShouldReturnViewModel()
    {
        var ownerId = "owner-id";
        var dealershipId = Guid.NewGuid();
        var dealership = new Dealership { Id = dealershipId, OwnerId = ownerId, Name = "Original Name" };
    
        var dealerships = new List<Dealership> { dealership };
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetEditDealershipAsync(dealershipId, ownerId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Original Name", result.Name);
    }

    [Test]
    public async Task GetEditDealershipAsync_WhenUserIsNotOwner_ShouldReturnNull()
    {
        var ownerId = "owner-id";
        var nonOwnerId = "non-owner-id";
        var dealershipId = Guid.NewGuid();
        var dealership = new Dealership { Id = dealershipId, OwnerId = ownerId, Name = "Original Name" };
    
        var dealerships = new List<Dealership> { dealership };
        var mockQueryable = dealerships.BuildMock();
        _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _dealerShipService.GetEditDealershipAsync(dealershipId, nonOwnerId);

        Assert.IsNull(result);
    }
    [Test]
public async Task EditDealershipAsync_WhenUserIsOwner_ShouldUpdateDealershipAndReturnTrue()
{
    var ownerId = "owner-id";
    var dealershipId = Guid.NewGuid();
    var model = new EditDealershipInputModel { Id = dealershipId, Name = "Updated Name" };
    var existingDealership = new Dealership { Id = dealershipId, OwnerId = ownerId, Name = "Original Name", Logo = new byte[1] };
    
    _mockUserManager.Setup(um => um.FindByIdAsync(ownerId)).ReturnsAsync(new ApplicationUser { Id = ownerId });
    
    var dealerships = new List<Dealership> { existingDealership };
    var mockQueryable = dealerships.BuildMock();
    _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

    var result = await _dealerShipService.EditDealershipAsync(ownerId, model, null);

    Assert.IsTrue(result);
    _mockDealershipRepository.Verify(r => r.UpdateAsync(It.Is<Dealership>(d => d.Name == "Updated Name")), Times.Once);
    _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
}

[Test]
public async Task EditDealershipAsync_WhenUserIsNotOwner_ShouldNotUpdateAndReturnFalse()
{
    var ownerId = "owner-id";
    var nonOwnerId = "non-owner-id";
    var dealershipId = Guid.NewGuid();
    var model = new EditDealershipInputModel { Id = dealershipId, Name = "Updated Name" };
    var existingDealership = new Dealership { Id = dealershipId, OwnerId = ownerId };
    
    _mockUserManager.Setup(um => um.FindByIdAsync(nonOwnerId)).ReturnsAsync(new ApplicationUser { Id = nonOwnerId });

    var dealerships = new List<Dealership> { existingDealership };
    var mockQueryable = dealerships.BuildMock();
    _mockDealershipRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

    var result = await _dealerShipService.EditDealershipAsync(nonOwnerId, model, null);
    
    Assert.IsFalse(result);
    _mockDealershipRepository.Verify(r => r.UpdateAsync(It.IsAny<Dealership>()), Times.Never);
    _mockDealershipRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
}

}