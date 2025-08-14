using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Services.Core.Contracts;
using MockQueryable;
using Moq;

namespace CarSellingPlatform.Services.Core.Tests;
[TestFixture]
public class CarComparisonTests
{
    private Mock<IRepository<Car, Guid>> _mockCarRepository;
    private ICarComparisonService _carComparisonService;
    
    [SetUp]
    public void SetUp()
    {
        _mockCarRepository = new Mock<IRepository<Car, Guid>>();
        _carComparisonService = new CarComparisonService(_mockCarRepository.Object);
    }
    [Test]
    public async Task GetAllCars_WhenCarsExist_ShouldReturnAllCars()
    {
        var cars = new List<Car>
        {
            new Car { Brand = new Brand(), Engine = new Engine(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission() },
            new Car { Brand = new Brand(), Engine = new Engine(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission() }
        };
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _carComparisonService.GetAllCars();
        
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task GetAllCars_WhenNoCarsExist_ShouldReturnEmptyCollection()
    {
        var cars = new List<Car>();
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _carComparisonService.GetAllCars();

        Assert.IsNotNull(result);
        Assert.IsEmpty(result);
    }
    [Test]
    public async Task GetCar_WithMatchingSearchTerm_ShouldReturnCorrectCar()
    {
        var cars = new List<Car>
        {
            new Car { Model = "M5", Year = 2024, Brand = new Brand { Name = "BMW" }, Engine = new Engine(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission() },
            new Car { Model = "A4", Year = 2023, Brand = new Brand { Name = "Audi" }, Engine = new Engine(), Category = new Category(), FuelType = new FuelType(), Transmission = new Transmission() }
        };
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _carComparisonService.GetCar("2024 bmw m5");

        Assert.IsNotNull(result);
        Assert.AreEqual("M5", result.Model);
    }
    [Test]
    public async Task GetCar_WithNonMatchingSearchTerm_ShouldReturnNull()
    {
        var cars = new List<Car>();
    
        var mockQueryable = cars.BuildMock();
        _mockCarRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

        var result = await _carComparisonService.GetCar("a non-matching term");
    
        Assert.IsNull(result);
        _mockCarRepository.Verify(r => r.GetAllAttached(), Times.Once);
    }
}