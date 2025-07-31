using System.Linq.Expressions;
using System.Text;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;

public class CarServiceTests
{
    private Mock<IRepository<Car,Guid>> _mockCarRepository;
    private Mock<IRepository<Engine, Guid>> _mockEngineRepository;
    private Mock<IRepository<UserCar, Guid>> _mockUserCarRepository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;
    private ICarService _carService;
    private List<Car> _testCars;
    private List<Engine> _testEngines;
    private const string TestUserId = "test-user-id";
    private readonly Guid _brandIdToyota = Guid.NewGuid();
    private readonly Guid _brandIdHonda = Guid.NewGuid();
    [SetUp]
    public void SetUp()
    {
        _mockCarRepository = new Mock<IRepository<Car, Guid>>();
        _mockEngineRepository = new Mock<IRepository<Engine, Guid>>();
        _mockUserCarRepository = new Mock<IRepository<UserCar, Guid>>();
        
        InitializeTestData();
        var mockQueryable = _testCars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        var mockEngineQueryable = _testEngines.BuildMock();
        _mockEngineRepository.Setup(r => r.GetAllAttached()).Returns(mockEngineQueryable);
        var store = new Mock<IUserStore<ApplicationUser>>();
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        _carService = new CarService(
            _mockCarRepository.Object,
            _mockEngineRepository.Object,
            _mockUserCarRepository.Object,
            _mockUserManager.Object
        );
    }
    private void InitializeTestData()
        {
            var brandToyota = new Brand { Id = Guid.NewGuid(), Name = "Toyota" };
            var brandHonda = new Brand { Id = Guid.NewGuid(), Name = "Honda" };

            var categorySedan = new Category { Name = "Sedan" };
            var fuelTypeGasoline = new FuelType { Type = "Gasoline" };
            var transmissionAuto = new Transmission { Type = "Automatic" };
            var engine2L = new Engine { Displacement = "2.0L", Horsepower = 180, Cylinders = 4 };
            var engine2_5L = new Engine { Displacement = "2.5L", Horsepower = 203, Cylinders = 4 };
            var seller = new ApplicationUser { Id = TestUserId.ToUpper(), PhoneNumber = "123-456-7890" };
            _testEngines = new List<Engine> { engine2_5L, engine2L };
            _testCars = new List<Car>
            {
                // Car 1: Belongs to the user, matches brand filter
                new Car
                {
                    Id = Guid.NewGuid(), Model = "Camry", Brand = brandToyota, BrandId = _brandIdToyota,
                    SellerId = TestUserId, Seller = seller,
                    Price = 25000, Year = 2022, Color = "Blue", ImageUrl = "camry.jpg", Description = "A reliable sedan.",
                    Category = categorySedan, FuelType = fuelTypeGasoline, Transmission = transmissionAuto, Engine = engine2_5L, EngineId = engine2_5L.Id ,
                    UserCars = new List<UserCar>()
                },
                
                // Car 2: Favorited by the user, matches brand filter
                new Car
                {
                    Id = Guid.NewGuid(), Model = "Corolla", Brand = brandToyota, BrandId = _brandIdToyota,
                    SellerId = "another-seller-id", 
                    Price = 21000, Year = 2023, Color = "White", ImageUrl = "corolla.jpg", Description = "Very economical.",
                    Category = categorySedan, FuelType = fuelTypeGasoline, Transmission = transmissionAuto, Engine = engine2L, EngineId = engine2L.Id ,
                    UserCars = new List<UserCar> { new UserCar { UserId = TestUserId.ToLower() } } // Test case insensitivity
                },
                // Car 3: Does not belong to or is favorited by the user
                new Car
                {
                    Id = Guid.NewGuid(), Model = "Civic", Brand = brandHonda, BrandId = _brandIdHonda,
                    SellerId = "another-seller-id-2", Seller = new ApplicationUser{ Id = "another-seller-id-2", PhoneNumber = "987-654-3210"},
                    Price = 22000, Year = 2021, Color = "Black", ImageUrl = "civic.jpg", Description = "Sporty and fun.",
                    Category = categorySedan, FuelType = fuelTypeGasoline, Transmission = transmissionAuto, Engine = engine2L, EngineId = engine2L.Id ,
                    UserCars = new List<UserCar>()
                },
                new Car
                {
                    Id = Guid.NewGuid(), Model = "Supra", Brand = brandToyota, BrandId = _brandIdToyota,
                    SellerId = TestUserId, Seller = seller,
                    Price = 25000, Year = 2022, Color = "Blue", ImageUrl = "camry.jpg", Description = "A reliable sedan.",
                    Category = categorySedan, FuelType = fuelTypeGasoline, Transmission = transmissionAuto, Engine = engine2_5L, EngineId = engine2_5L.Id ,
                    UserCars = new List<UserCar>()
                },
            };
        }
    [Test]
    public async Task ListPagedAsync_ShouldReturnCorrectlyPagedData_WhenNoFiltersApplied()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 2;

        // Act
        var result = await _carService.ListPagedAsync(TestUserId, pageNumber, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<PagedListViewModel<IndexCarViewModel>>(result);
        Assert.AreEqual(pageNumber, result.PageNumber);
        Assert.AreEqual(2, result.Items.Count()); // Should be pageSize
        Assert.AreEqual(2, result.TotalPages); // 3 items total / 2 per page = 2 pages

        var firstCar = result.Items.First();
        Assert.AreEqual("Camry", firstCar.CarModel);
        Assert.IsTrue(firstCar.IsUserSeller);
        Assert.IsFalse(firstCar.IsUserFavorite);

        var secondCar = result.Items.Skip(1).First();
        Assert.AreEqual("Corolla", secondCar.CarModel);
        Assert.IsFalse(secondCar.IsUserSeller);
        Assert.IsTrue(secondCar.IsUserFavorite);
    }
    [Test]
    public async Task ListPagedAsync_ShouldReturnFilteredData_WhenSearchTermIsProvided()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 5;
        var searchTerm = "camry"; // Lowercase to test case-insensitivity

        // Act
        var result = await _carService.ListPagedAsync(TestUserId, pageNumber, pageSize, search: searchTerm);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Items.Count());
        Assert.AreEqual(1, result.TotalPages);

        var car = result.Items.First();
        Assert.AreEqual("Camry", car.CarModel);
        Assert.AreEqual("Toyota", car.BrandName);
    }
    [Test]
    public async Task ListPagedAsync_ShouldReturnEmptyResult_WhenNoCarsMatchFilter()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 5;
        var searchTerm = "non-existent-model";

        // Act
        var result = await _carService.ListPagedAsync(TestUserId, pageNumber, pageSize, search: searchTerm);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Items);
        Assert.AreEqual(0, result.TotalPages);
        Assert.AreEqual(pageNumber, result.PageNumber);
    }
    [Test]
        public async Task AddCarAsync_WithValidDataAndImage_ShouldAddCarAndEngineAndReturnTrue()
        {
            // Arrange
            const string validUserId = "valid-user-id";
            var capturedCar = (Car)null;
            var capturedEngine = (Engine)null;

            var model = new AddCarViewModel
            {
                BrandId = Guid.NewGuid(), CarModel = "Test Model", Price = 15000, Year = 2024,
                Color = "Black", Description = "A great test car", CategoryId = Guid.NewGuid(),
                TransmissionId = Guid.NewGuid(), FuelTypeId = Guid.NewGuid(), Horsepower = 250,
                Displacement = "2.0L", Cylinders = 4, EngineCode = "T-ENG"
            };

            var imageContent = "fake image data";
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(imageContent));
            var mockImageFile = new Mock<IFormFile>();
            mockImageFile.Setup(f => f.Length).Returns(memoryStream.Length);
            mockImageFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Callback<Stream, CancellationToken>((stream, _) => memoryStream.CopyTo(stream))
                .Returns(Task.CompletedTask);

            _mockUserManager.Setup(um => um.FindByIdAsync(validUserId)).ReturnsAsync(new ApplicationUser { Id = validUserId });
            _mockEngineRepository.Setup(r => r.AddAsync(It.IsAny<Engine>())).Callback<Engine>(e => capturedEngine = e).Returns(Task.CompletedTask);
            _mockCarRepository.Setup(r => r.AddAsync(It.IsAny<Car>())).Callback<Car>(c => capturedCar = c).Returns(Task.CompletedTask);
            _mockCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _carService.AddCarAsync(validUserId, model, mockImageFile.Object);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(capturedEngine);
            Assert.AreEqual(model.Horsepower, capturedEngine.Horsepower);
            Assert.IsNotNull(capturedCar);
            Assert.AreEqual(model.CarModel, capturedCar.Model);
            Assert.AreEqual(validUserId, capturedCar.SellerId);
            Assert.AreEqual(capturedEngine.Id, capturedCar.EngineId);
            Assert.AreEqual(imageContent, Encoding.UTF8.GetString(capturedCar.ImageData));
            
            _mockUserManager.Verify(um => um.FindByIdAsync(validUserId), Times.Once);
            _mockEngineRepository.Verify(r => r.AddAsync(It.IsAny<Engine>()), Times.Once);
            _mockCarRepository.Verify(r => r.AddAsync(It.IsAny<Car>()), Times.Once);
            _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    [Test]
    public async Task AddCarAsync_WithInvalidUserId_ShouldNotAddEntitiesAndReturnFalse()
    {
        // Arrange
        const string invalidUserId = "invalid-user-id";
        var model = new AddCarViewModel();

        _mockUserManager.Setup(um => um.FindByIdAsync(invalidUserId)).ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _carService.AddCarAsync(invalidUserId, model, null);

        // Assert
        Assert.IsFalse(result);
        _mockUserManager.Verify(um => um.FindByIdAsync(invalidUserId), Times.Once);
        _mockEngineRepository.Verify(r => r.AddAsync(It.IsAny<Engine>()), Times.Never);
        _mockCarRepository.Verify(r => r.AddAsync(It.IsAny<Car>()), Times.Never);
        _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetDetailsCarAsync_WithValidId_ShouldReturnCorrectDetailsViewModel()
    {
        // Arrange
        var carToTest = _testCars.First(c => c.Model == "Camry");
        var carId = carToTest.Id;
            
        // Act
        var result = await _carService.GetDetailsCarAsync(carId, TestUserId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<DetailsCarViewModel>(result);

        Assert.AreEqual(carToTest.Id, result.Id);
        Assert.AreEqual(carToTest.Model, result.CarModel);
        Assert.AreEqual(carToTest.Brand.Name, result.BrandName);
        Assert.AreEqual(carToTest.Engine.Horsepower, result.HorsePower);
        Assert.AreEqual(carToTest.Seller.PhoneNumber, result.PhoneNubmer);
        Assert.IsTrue(result.IsUserSeller);
    }
    [Test]
    public async Task GetDetailsCarAsync_WithInvalidOrNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _carService.GetDetailsCarAsync(nonExistentId, TestUserId);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public async Task GetEditCarAsync_WithValidIdAndCorrectOwner_ShouldReturnViewModel()
    {
        // Arrange
        var carToTest = _testCars.First();
        var carId = carToTest.Id;
        var ownerId = TestUserId;

        // Act
        var result = await _carService.GetEditCarAsync(carId, ownerId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<EditCarViewModel>(result);
        Assert.AreEqual(carId, result.Id);
        Assert.AreEqual(carToTest.Model, result.CarModel);
        Assert.AreEqual(ownerId, result.SellerId); // Check if correct sellerId is assigned
    }
    [Test]
    public async Task GetEditCarAsync_WithValidIdButWrongOwner_ShouldReturnNull()
    {
        // Arrange
        var carToTest = _testCars.First();
        var carId = carToTest.Id;
        var wrongOwnerId = "this-is-not-the-owner";

        // Act
        var result = await _carService.GetEditCarAsync(carId, wrongOwnerId);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public async Task EditCarAsync_WithValidData_ShouldUpdateCarAndEngineAndReturnTrue()
    {
        // Arrange
        var carToUpdate = _testCars.First();
        var model = new EditCarViewModel
        {
            Id = carToUpdate.Id,
            CarModel = "Camry Updated", 
            Year = 2023,                
            Horsepower = 210,           
            EngineCode = "A25A-FXS"     
        };

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId))
            .ReturnsAsync(new ApplicationUser { Id = TestUserId });

        _mockCarRepository.Setup(r => r.UpdateAsync(It.IsAny<Car>())).Returns(Task.FromResult(true));
        _mockEngineRepository.Setup(r => r.UpdateAsync(It.IsAny<Engine>())).Returns(Task.FromResult(true));
        _mockCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _carService.EditCarAsync(TestUserId, model, null);

        // Assert
        Assert.IsTrue(result);

        // Verify the original object in our list was modified
        Assert.AreEqual("Camry Updated", carToUpdate.Model);
        Assert.AreEqual(2023, carToUpdate.Year);
        Assert.AreEqual(210, carToUpdate.Engine.Horsepower);

        // Verify repository methods were called
        _mockCarRepository.Verify(r => r.UpdateAsync(It.IsAny<Car>()), Times.Once);
        _mockEngineRepository.Verify(r => r.UpdateAsync(It.IsAny<Engine>()), Times.Once);
        _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task EditCarAsync_WithNonExistentCarId_ShouldReturnFalse()
    {
        // Arrange
        var model = new EditCarViewModel { Id = Guid.NewGuid() }; // This ID does not exist

        // Act
        var result = await _carService.EditCarAsync(TestUserId, model, null);
            
        // Assert
        Assert.IsFalse(result);

        // Verify no update or save operations were ever called
        _mockCarRepository.Verify(r => r.UpdateAsync(It.IsAny<Car>()), Times.Never);
        _mockEngineRepository.Verify(r => r.UpdateAsync(It.IsAny<Engine>()), Times.Never);
        _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetDeleteCarAsync_WithValidIdAndCorrectOwner_ShouldReturnViewModel()
    {
        // Arrange
        var carToTest = _testCars.First();
        var carId = carToTest.Id;
        var ownerId = TestUserId;

        // Act
        var result = await _carService.GetDeleteCarAsync(carId, ownerId);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<DeleteCarViewModel>(result);
        Assert.AreEqual(carId, result.Id);
        Assert.AreEqual(carToTest.Model, result.CarModel);
        Assert.AreEqual(ownerId, result.SellerId);
        Assert.AreEqual(carToTest.Seller.UserName, result.Seller);
    }
    [Test]
    public async Task GetDeleteCarAsync_WithValidIdButWrongOwner_ShouldReturnNull()
    {
        // Arrange
        var carToTest = _testCars.First();
        var carId = carToTest.Id;
        var wrongOwnerId = "this-is-not-the-owner";

        // Act
        var result = await _carService.GetDeleteCarAsync(carId, wrongOwnerId);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public async Task SoftDeleteCarAsync_WithExistingCarAndCorrectOwner_ShouldSetIsDeletedAndReturnTrue()
    {
        // Arrange
        var carToDelete = _testCars.First();
        var model = new DeleteCarViewModel { Id = carToDelete.Id };
        var ownerId = TestUserId;

        _mockUserManager.Setup(um => um.FindByIdAsync(ownerId))
            .ReturnsAsync(new ApplicationUser { Id = ownerId });
        // Mock the specific repository call used by the method
        _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync(carToDelete);
            
        _mockCarRepository.Setup(r => r.UpdateAsync(It.IsAny<Car>())).Returns(Task.FromResult(true));
        _mockCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _carService.SoftDeleteCarAsync(model, ownerId);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(carToDelete.IsDeleted); // Verify the flag was set
        _mockCarRepository.Verify(r => r.UpdateAsync(carToDelete), Times.Once);
        _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task SoftDeleteCarAsync_WithNonExistentCar_ShouldReturnFalse()
    {
        // Arrange
        var model = new DeleteCarViewModel { Id = Guid.NewGuid() }; // This ID does not exist

        // Mock the repository to find nothing
        _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync((Car)null);

        // Act
        var result = await _carService.SoftDeleteCarAsync(model, TestUserId);

        // Assert
        Assert.IsFalse(result);
            
        // Verify no update or save operations were called
        _mockCarRepository.Verify(r => r.UpdateAsync(It.IsAny<Car>()), Times.Never);
        _mockCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task GetFavoriteCarsAsync_WithValidUser_ShouldReturnOnlyTheirFavoriteCars()
    {
        // Arrange
        var user = new ApplicationUser { Id = TestUserId };
        var car1 = new Car { Id = Guid.NewGuid(), Model = "Favorite Car 1", Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission(), Engine = new Engine() };
        var car2 = new Car { Id = Guid.NewGuid(), Model = "Favorite Car 2", Brand = new Brand(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission(), Engine = new Engine() };
        var otherUserCar = new Car { Id = Guid.NewGuid(), Model = "Another User's Car" };
            
        var favorites = new List<UserCar>
        {
            new UserCar { UserId = TestUserId, CarId = car1.Id, Car = car1 },
            new UserCar { UserId = TestUserId, CarId = car2.Id, Car = car2 },
            new UserCar { UserId = "other-user", CarId = otherUserCar.Id, Car = otherUserCar }
        };

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
        var mockQueryable = favorites.BuildMock();
        _mockUserCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        // Act
        var result = await _carService.GetFavoriteCarsAsync(TestUserId, 1, 5);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count()); // Should only be 2, not 3
        Assert.AreEqual(1, result.TotalPages);   // Total pages based on 2 items, not 3
        Assert.IsTrue(result.Items.Any(c => c.CarModel == "Favorite Car 1"));
        Assert.IsFalse(result.Items.Any(c => c.CarModel == "Another User's Car"));
    }
    [Test]
    public async Task GetFavoriteCarsAsync_WithInvalidUser_ShouldReturnNull()
    {
        // Arrange
        var invalidUserId = "invalid-user";
        _mockUserManager.Setup(um => um.FindByIdAsync(invalidUserId)).ReturnsAsync((ApplicationUser)null);
        var emptyFavoritesList = new List<UserCar>();
        var mockQueryable = emptyFavoritesList.BuildMock(); // Use .BuildMock()
        _mockUserCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);
        // Act
        var result = await _carService.GetFavoriteCarsAsync(invalidUserId, 1, 5);

        // Assert
        Assert.IsNull(result);
    }
    [Test]
    public async Task AddCarToFavoritesAsync_WhenCarIsNotOwnedAndNotFavorite_ShouldReturnTrue()
    {
        // Arrange
        var user = new ApplicationUser { Id = TestUserId };
        var carToAdd = _testCars.First(c => c.Model == "Corolla"); // A car not owned by the user

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockCarRepository.Setup(r => r.GetByIdAsync(carToAdd.Id)).ReturnsAsync(carToAdd);

        // Setup to simulate the car is NOT already a favorite
        _mockUserCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<UserCar, bool>>>()))
            .ReturnsAsync((UserCar)null);

        _mockUserCarRepository.Setup(r => r.AddAsync(It.IsAny<UserCar>())).Returns(Task.CompletedTask);
        _mockUserCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _carService.AddCarToFavoritesAsync(TestUserId, carToAdd.Id);

        // Assert
        Assert.IsTrue(result);
        _mockUserCarRepository.Verify(r => r.AddAsync(It.IsAny<UserCar>()), Times.Once);
        _mockUserCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task AddCarToFavoritesAsync_WhenUserIsSeller_ShouldReturnFalse()
    {
        // Arrange
        var user = new ApplicationUser { Id = TestUserId };
        var carToFail = _testCars.First(c => c.Model == "Camry"); // The car owned by the user

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockCarRepository.Setup(r => r.GetByIdAsync(carToFail.Id)).ReturnsAsync(carToFail);

        // Act
        var result = await _carService.AddCarToFavoritesAsync(TestUserId, carToFail.Id);

        // Assert
        Assert.IsFalse(result);
        _mockUserCarRepository.Verify(r => r.AddAsync(It.IsAny<UserCar>()), Times.Never);
        _mockUserCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task RemoveCarFromFavoritesAsync_WhenCarIsFavorite_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var user = new ApplicationUser { Id = TestUserId };
        var car = new Car { Id = Guid.NewGuid() };
        var existingFavorite = new UserCar { UserId = TestUserId, CarId = car.Id };

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync(car);

        // Setup to simulate that the car IS currently a favorite
        _mockUserCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<UserCar, bool>>>()))
            .ReturnsAsync(existingFavorite);
            
        // HardDelete is synchronous, so we don't need to set up a return task
        _mockUserCarRepository.Setup(r => r.HardDelete(It.IsAny<UserCar>()));
        _mockUserCarRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _carService.RemoveCarFromFavoritesAsync(TestUserId, car.Id);

        // Assert
        Assert.IsTrue(result);
        _mockUserCarRepository.Verify(r => r.HardDelete(existingFavorite), Times.Once);
        _mockUserCarRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Test]
    public async Task RemoveCarFromFavoritesAsync_WhenCarIsNotFavorite_ShouldReturnFalse()
    {
        // Arrange
        var user = new ApplicationUser { Id = TestUserId };
        var car = new Car { Id = Guid.NewGuid() };

        _mockUserManager.Setup(um => um.FindByIdAsync(TestUserId)).ReturnsAsync(user);
        _mockCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync(car);

        // Setup to simulate that the car is NOT a favorite
        _mockUserCarRepository.Setup(r => r.SingleOrDefaultAsync(It.IsAny<Expression<Func<UserCar, bool>>>()))
            .ReturnsAsync((UserCar)null);

        // Act
        var result = await _carService.RemoveCarFromFavoritesAsync(TestUserId, car.Id);

        // Assert
        Assert.IsFalse(result);

        // Verify the delete methods were never called
        _mockUserCarRepository.Verify(r => r.HardDelete(It.IsAny<UserCar>()), Times.Never);
        _mockUserCarRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
    [Test]
    public async Task MyCarsPagedAsync_WithUserWhoHasCars_ShouldReturnOnlyTheirCars()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var result = await _carService.MyCarsPagedAsync(TestUserId, pageNumber, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Items.Count()); // Should only be the 2 cars sold by TestUserId
        Assert.AreEqual(1, result.TotalPages);
        Assert.IsTrue(result.Items.All(c => _testCars.Where(tc => tc.SellerId == TestUserId).Select(tc => tc.Id).Contains(c.Id)));
        Assert.IsFalse(result.Items.Any(c => c.CarModel == "Civic"));
    }
    [Test]
    public async Task MyCarsPagedAsync_WithUserWhoHasNoCars_ShouldReturnEmptyList()
    {
        // Arrange
        var userIdWithNoCars = "user-with-no-cars";
        var pageNumber = 1;
        var pageSize = 5;

        // Act
        var result = await _carService.MyCarsPagedAsync(userIdWithNoCars, pageNumber, pageSize);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result.Items);
        Assert.AreEqual(0, result.TotalPages);
        Assert.AreEqual(pageNumber, result.PageNumber);
    }
    [Test]
    public async Task GetCarImageByIdAsync_WhenCarHasImage_ShouldReturnImageDataAndContentType()
    {
        // Arrange
        var carId = Guid.NewGuid();
        var sampleImageData = new byte[] { 1, 2, 3, 4, 5 };
        var carWithImage = new Car { Id = carId, ImageData = sampleImageData };

        _mockCarRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync(carWithImage);

        // Act
        var result = await _carService.GetCarImageByIdAsync(carId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(sampleImageData, result.Value.ImageData);
        Assert.AreEqual("image/jpeg", result.Value.ContentType);
    }
    [Test]
    public async Task GetCarImageByIdAsync_WhenCarHasNoImage_ShouldReturnNull()
    {
        // Arrange
        var carId = Guid.NewGuid();
        // This car object has a null ImageData property
        var carWithoutImage = new Car { Id = carId, ImageData = null };

        _mockCarRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Car, bool>>>()))
            .ReturnsAsync(carWithoutImage);

        // Act
        var result = await _carService.GetCarImageByIdAsync(carId);

        // Assert
        Assert.IsNull(result);
    }
}