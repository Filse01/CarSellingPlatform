using System.Linq.Expressions;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Services.Core.Tests.Helpers;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable;
using Moq;
namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class CarManagementTests
{
    private Mock<IRepository<Car, Guid>> _mockCarRepository;
    private Mock<IRepository<Engine, Guid>> _mockEngineRepository;
    private ICarManagementService _carManagementService;

    [SetUp]
    public void SetUp()
    {
        _mockCarRepository = new Mock<IRepository<Car, Guid>>();
        _mockEngineRepository = new Mock<IRepository<Engine, Guid>>();
        _carManagementService = new CarManagementService(
            _mockCarRepository.Object,
            _mockEngineRepository.Object,
            null
        );
    }
    [TearDown]
    public void TearDown()
    {
        _carManagementService = null;
        _mockCarRepository = null;
        _mockEngineRepository = null;
    }
    [Test]
    public async Task ListPagedAsync_ValidData_ReturnsCorrectPagedResult()
    {
        var userId = "user123";
        var pageNumber = 1;
        var pageSize = 2;
    
        var brandId1 = Guid.NewGuid();
        var brandId2 = Guid.NewGuid();
        var sellerId1 = Guid.NewGuid();
        var sellerId2 = Guid.NewGuid();
        var sellerId3 = Guid.NewGuid();
        List<Car> cars = new List<Car>
        {
            new Car
            {
                Id = Guid.NewGuid(),
                Model = "Camry",
                Year = 2022,
                Brand = new Brand { Id = brandId1, Name = "Toyota" },
                Seller = new ApplicationUser()
                    { Id = sellerId1.ToString(), FirstName = "John", LastName = "Doe", Email = "john.doe@email.com" }
            },
            new Car
            {
                Id = Guid.NewGuid(),
                Model = "Civic",
                Year = 2021,
                Brand = new Brand { Id = brandId2, Name = "Honda" },
                Seller = new ApplicationUser
                {
                    Id = sellerId2.ToString(), FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com"
                }
            },
            new Car
            {
                Id = Guid.NewGuid(),
                Model = "Corolla",
                Year = 2023,
                Brand = new Brand { Id = brandId1, Name = "Toyota" },
                Seller = new ApplicationUser
                {
                    Id = sellerId3.ToString(), FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@email.com"
                }
            }
        };
        IQueryable<Car> queryableCars = cars.BuildMock();
        this._mockCarRepository
            .Setup(r => r.GetAllAttached())
            .Returns(queryableCars);
        var result = await _carManagementService.ListPagedAsync(userId, pageNumber, pageSize);
        Assert.IsNotNull(result);
        Assert.AreEqual(pageNumber, result.PageNumber);
        Assert.AreEqual(2, result.TotalPages);
        Assert.AreEqual(2, result.Items.Count());
        var itemsList = result.Items.ToList();
        var firstCar = itemsList[0];
        Assert.AreEqual("Camry", firstCar.Model);
        Assert.AreEqual("Toyota", firstCar.Brand);
        Assert.AreEqual(2022, firstCar.Year);
        Assert.AreEqual("John Doe", firstCar.SellerName);
        Assert.AreEqual("john.doe@email.com", firstCar.Email);
        
        var secondCar = itemsList[1];
        Assert.AreEqual("Civic", secondCar.Model);
        Assert.AreEqual("Honda", secondCar.Brand);
        Assert.AreEqual(2021, secondCar.Year);
        Assert.AreEqual("Jane Smith", secondCar.SellerName);
        Assert.AreEqual("jane.smith@email.com", secondCar.Email);

        _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
    }
    [Test]
    public async Task ListPagedAsync_EmptyData_ReturnsEmptyPagedResult()
    {
        
        var userId = "user123";
        var pageNumber = 1;
        var pageSize = 10;
    
        var cars = new List<Car>();
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        
        var result = await _carManagementService.ListPagedAsync(userId, pageNumber, pageSize);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.PageNumber);
        Assert.AreEqual(0, result.TotalPages);
        Assert.AreEqual(0, result.Items.Count());

        _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
    }
    [Test]
public async Task GetEditCarAsync_ValidIdAndUserId_ReturnsCorrectEditCarViewModel()
{
    
    var carId = Guid.NewGuid();
    var userId = Guid.NewGuid().ToString();
    var brandId = Guid.NewGuid();
    var transmissionId = Guid.NewGuid();
    var fuelTypeId = Guid.NewGuid();
    var categoryId = Guid.NewGuid();
    var engineId = Guid.NewGuid();
    
    var car = new Car
    {
        Id = carId,
        SellerId = userId,
        BrandId = brandId,
        Model = "Camry",
        Description = "Great car",
        Price = 25000,
        Color = "Red",
        TransmissionId = transmissionId,
        FuelTypeId = fuelTypeId,
        Year = 2022,
        ImageUrl = "image.jpg",
        CategoryId = categoryId,
        EngineId = engineId
    };
    
    var engine = new Engine
    {
        Id = engineId,
        Displacement = "2.0L",
        Cylinders = 4,
        Horsepower = 200,
        EngineCode = "2GR-FE"
    };
    
    var cars = new List<Car> { car };
    var engines = new List<Engine> { engine };
    
    var mockCarQueryable = cars.BuildMock();
    var mockEngineQueryable = engines.BuildMock();
    
    _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockCarQueryable);
    _mockEngineRepository.Setup(r => r.GetAllAttached()).Returns(mockEngineQueryable);
    
    var result = await _carManagementService.GetEditCarAsync(carId, userId);
    
    Assert.IsNotNull(result);
    Assert.AreEqual(carId, result.Id);
    Assert.AreEqual(brandId, result.BrandId);
    Assert.AreEqual("Camry", result.CarModel);
    Assert.AreEqual("Great car", result.Description);
    Assert.AreEqual(25000, result.Price);
    Assert.AreEqual("Red", result.Color);
    Assert.AreEqual(transmissionId, result.TransmissionId);
    Assert.AreEqual(fuelTypeId, result.FuelTypeId);
    Assert.AreEqual(2022, result.Year);
    Assert.AreEqual("2.0L", result.Displacement.ToString());
    Assert.AreEqual(200, result.Horsepower);
    Assert.AreEqual("2GR-FE", result.EngineCode);
    Assert.AreEqual("image.jpg", result.ImageUrl);
    Assert.AreEqual(userId, result.SellerId);
    Assert.AreEqual(4, result.Cylinders);
    Assert.AreEqual(categoryId, result.CategoryId);
    Assert.AreEqual(engineId, result.EngineId);

    _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
    _mockEngineRepository.Verify(r => r.GetAllAttached(), Times.Once);
}
    [Test]
    public async Task GetEditCarAsync_ValidIdButDifferentUserId_ReturnsNull()
    {
        
        var carId = Guid.NewGuid();
        var actualUserId = Guid.NewGuid().ToString();
        var requestingUserId = Guid.NewGuid().ToString();
        var engineId = Guid.NewGuid();
    
        var car = new Car
        {
            Id = carId,
            SellerId = actualUserId,
            Model = "Camry",
            EngineId = engineId
        };
    
        var engine = new Engine
        {
            Id = engineId,
            Displacement = "2.0L",
            Cylinders = 4,
            Horsepower = 200,
            EngineCode = "2GR-FE"
        };
    
        var cars = new List<Car> { car };
        var engines = new List<Engine> { engine };
    
        var mockCarQueryable = cars.BuildMock();
        var mockEngineQueryable = engines.BuildMock();
    
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockCarQueryable);
        _mockEngineRepository.Setup(r => r.GetAllAttached()).Returns(mockEngineQueryable);
        
        var result = await _carManagementService.GetEditCarAsync(carId, requestingUserId);
        
        Assert.IsNull(result);

        _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
        _mockEngineRepository.Verify(r => r.GetAllAttached(), Times.Once);
    }
[Test]
public async Task EditCarAsync_ValidData_UpdatesCarAndEngineAndReturnsTrue()
{
    
    var userId = Guid.NewGuid().ToString();
    var carId = Guid.NewGuid();
    var engineId = Guid.NewGuid();
    var brandId = Guid.NewGuid();
    var transmissionId = Guid.NewGuid();
    var fuelTypeId = Guid.NewGuid();
    var categoryId = Guid.NewGuid();
    
    var user = new ApplicationUser { Id = userId };
    
    var existingCar = new Car
    {
        Id = carId,
        Model = "Old Model",
        ImageUrl = "old-image.jpg",
        Description = "Old description",
        Price = 20000,
        Color = "Blue",
        TransmissionId = Guid.NewGuid(),
        FuelTypeId = Guid.NewGuid(),
        Year = 2020,
        BrandId = Guid.NewGuid(),
        CategoryId = Guid.NewGuid(),
        EngineId = engineId
    };
    
    var existingEngine = new Engine
    {
        Id = engineId,
        Cylinders = 4,
        Horsepower = 150,
        EngineCode = "OLD123"
    };
    
    var model = new EditCarViewModel
    {
        Id = carId,
        CarModel = "Updated Model",
        ImageUrl = "new-image.jpg",
        Description = "Updated description",
        Price = 25000,
        Color = "Red",
        TransmissionId = transmissionId,
        FuelTypeId = fuelTypeId,
        Year = 2023,
        BrandId = brandId,
        CategoryId = categoryId,
        Cylinders = 6,
        Horsepower = 200,
        EngineCode = "NEW456"
    };
    
    var cars = new List<Car> { existingCar };
    var engines = new List<Engine> { existingEngine };
    
    var mockCarQueryable = cars.BuildMock();
    var mockEngineQueryable = engines.BuildMock();
    var userManagerMock = new Mock<UserManager<ApplicationUser>>(
        new Mock<IUserStore<ApplicationUser>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<IPasswordHasher<ApplicationUser>>().Object,
        new IUserValidator<ApplicationUser>[0],
        new IPasswordValidator<ApplicationUser>[0],
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    var carManagementService = new CarManagementService(
        _mockCarRepository.Object,
        _mockEngineRepository.Object,
        userManagerMock.Object
    );
    
    userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
    _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockCarQueryable);
    _mockEngineRepository.Setup(r => r.GetAllAttached()).Returns(mockEngineQueryable);
    _mockCarRepository.Setup(r => r.UpdateAsync(It.IsAny<Car>())).ReturnsAsync(true);
    _mockEngineRepository.Setup(r => r.UpdateAsync(It.IsAny<Engine>())).ReturnsAsync(true);
    _mockCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
    _mockEngineRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

    
    var result = await carManagementService.EditCarAsync(userId, model);

    
    Assert.IsTrue(result);

    // Verify car was updated with correct values
    Assert.AreEqual("Updated Model", existingCar.Model);
    Assert.AreEqual("new-image.jpg", existingCar.ImageUrl);
    Assert.AreEqual("Updated description", existingCar.Description);
    Assert.AreEqual(25000, existingCar.Price);
    Assert.AreEqual("Red", existingCar.Color);
    Assert.AreEqual(transmissionId, existingCar.TransmissionId);
    Assert.AreEqual(fuelTypeId, existingCar.FuelTypeId);
    Assert.AreEqual(2023, existingCar.Year);
    Assert.AreEqual(brandId, existingCar.BrandId);
    Assert.AreEqual(categoryId, existingCar.CategoryId);
    
    // Verify engine was updated with correct values
    Assert.AreEqual(6, existingEngine.Cylinders);
    Assert.AreEqual(200, existingEngine.Horsepower);
    Assert.AreEqual("NEW456", existingEngine.EngineCode);

    // Verify all repository methods were called
    userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
    _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
    _mockEngineRepository.Verify(r => r.GetAllAttached(), Times.Once);
    _mockCarRepository.Verify(r => r.UpdateAsync(existingCar), Times.Once);
    _mockEngineRepository.Verify(r => r.UpdateAsync(existingEngine), Times.Once);
    _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    _mockEngineRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
}
[Test]
public async Task SoftDeleteCarAsync_ValidData_SetsIsDeletedTrueAndReturnsTrue()
{
    
    var userId = "user-123";
    var carId = Guid.NewGuid();
    
    var user = new ApplicationUser { Id = userId };
    
    var existingCar = new Car
    {
        Id = carId,
        Model = "Test Car",
        IsDeleted = false
    };
    var userManagerMock = new Mock<UserManager<ApplicationUser>>(
        new Mock<IUserStore<ApplicationUser>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<IPasswordHasher<ApplicationUser>>().Object,
        new IUserValidator<ApplicationUser>[0],
        new IPasswordValidator<ApplicationUser>[0],
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    var carManagementService = new CarManagementService(
        _mockCarRepository.Object,
        _mockEngineRepository.Object,
        userManagerMock.Object
    );
    
    userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
    _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
        .ReturnsAsync(existingCar);
    _mockCarRepository.Setup(r => r.UpdateAsync(It.IsAny<Car>())).ReturnsAsync(true);
    _mockCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

    
    var result = await carManagementService.SoftDeleteCarAsync(carId, userId);

    
    Assert.IsTrue(result);
    Assert.IsTrue(existingCar.IsDeleted);
    
    userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
    _mockCarRepository.Verify(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()), Times.Once);
    _mockCarRepository.Verify(r => r.UpdateAsync(existingCar), Times.Once);
    _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
}
[Test]
public async Task SoftDeleteCarAsync_CarNotFound_ReturnsFalse()
{
    
    var userId = "user-123";
    var carId = Guid.NewGuid();
    
    var user = new ApplicationUser { Id = userId };
    var userManagerMock = new Mock<UserManager<ApplicationUser>>(
        new Mock<IUserStore<ApplicationUser>>().Object,
        new Mock<IOptions<IdentityOptions>>().Object,
        new Mock<IPasswordHasher<ApplicationUser>>().Object,
        new IUserValidator<ApplicationUser>[0],
        new IPasswordValidator<ApplicationUser>[0],
        new Mock<ILookupNormalizer>().Object,
        new Mock<IdentityErrorDescriber>().Object,
        new Mock<IServiceProvider>().Object,
        new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    var carManagementService = new CarManagementService(
        _mockCarRepository.Object,
        _mockEngineRepository.Object,
        userManagerMock.Object
    );
    userManagerMock.Setup(um => um.FindByIdAsync(userId)).ReturnsAsync(user);
    _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
        .ReturnsAsync((Car)null); 

    
    var result = await carManagementService.SoftDeleteCarAsync(carId, userId);

    
    Assert.IsFalse(result);
    
    userManagerMock.Verify(um => um.FindByIdAsync(userId), Times.Once);
    _mockCarRepository.Verify(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()), Times.Once);
    _mockCarRepository.Verify(r => r.UpdateAsync(It.IsAny<Car>()), Times.Never);
    _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
}
}