using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Services.Core;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace CarSellingPlatform.Services.Core.Tests
{
    [TestFixture]
    public class CarInfoServiceTests
    {
        private Mock<IRepository<Brand, Guid>> _mockBrandRepository;
        private Mock<IRepository<Category, Guid>> _mockCategoryRepository;
        private Mock<IRepository<FuelType, Guid>> _mockFuelTypeRepository;
        private Mock<IRepository<Transmission, Guid>> _mockTransmissionRepository;
        private ICarInfoService _carInfoService;

        [SetUp]
        public void SetUp()
        {
            _mockBrandRepository = new Mock<IRepository<Brand, Guid>>();
            _mockCategoryRepository = new Mock<IRepository<Category, Guid>>();
            _mockFuelTypeRepository = new Mock<IRepository<FuelType, Guid>>();
            _mockTransmissionRepository = new Mock<IRepository<Transmission, Guid>>();

            _carInfoService = new CarInfoService(
                null, // Context is not used in the service methods, so we can pass null
                _mockBrandRepository.Object,
                _mockCategoryRepository.Object,
                _mockFuelTypeRepository.Object,
                _mockTransmissionRepository.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _carInfoService = null;
            _mockBrandRepository = null;
            _mockCategoryRepository = null;
            _mockFuelTypeRepository = null;
            _mockTransmissionRepository = null;
        }

        #region Constructor Tests

        [Test]
        public void Constructor_ValidParameters_CreatesInstance()
        {
            // Arrange & Act
            var service = new CarInfoService(
                null, // Context can be null since it's not used in the service methods
                _mockBrandRepository.Object,
                _mockCategoryRepository.Object,
                _mockFuelTypeRepository.Object,
                _mockTransmissionRepository.Object);

            // Assert
            Assert.IsNotNull(service);
            Assert.IsInstanceOf<ICarInfoService>(service);
        }

        [Test]
        public void Constructor_NullContext_DoesNotThrow()
        {
            // Arrange, Act & Assert - Context is not used, so null is acceptable
            Assert.DoesNotThrow(() => new CarInfoService(
                null,
                _mockBrandRepository.Object,
                _mockCategoryRepository.Object,
                _mockFuelTypeRepository.Object,
                _mockTransmissionRepository.Object));
        }

        [Test]
        public void Constructor_NullBrandRepository_DoesNotThrowButWillFailOnMethodCall()
        {
            // Arrange, Act & Assert - Constructor doesn't validate, but method calls will fail
            CarInfoService service = null;
            Assert.DoesNotThrow(() => service = new CarInfoService(
                null,
                null,
                _mockCategoryRepository.Object,
                _mockFuelTypeRepository.Object,
                _mockTransmissionRepository.Object));

            // Verify that calling the method with null repository throws
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetBrandsAsync());
        }

        [Test]
        public void Constructor_NullCategoryRepository_DoesNotThrowButWillFailOnMethodCall()
        {
            // Arrange, Act & Assert - Constructor doesn't validate, but method calls will fail
            CarInfoService service = null;
            Assert.DoesNotThrow(() => service = new CarInfoService(
                null,
                _mockBrandRepository.Object,
                null,
                _mockFuelTypeRepository.Object,
                _mockTransmissionRepository.Object));

            // Verify that calling the method with null repository throws
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetCategoriesAsync());
        }

        [Test]
        public void Constructor_NullFuelTypeRepository_DoesNotThrowButWillFailOnMethodCall()
        {
            // Arrange, Act & Assert - Constructor doesn't validate, but method calls will fail
            CarInfoService service = null;
            Assert.DoesNotThrow(() => service = new CarInfoService(
                null,
                _mockBrandRepository.Object,
                _mockCategoryRepository.Object,
                null,
                _mockTransmissionRepository.Object));

            // Verify that calling the method with null repository throws
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetFuelTypesAsync());
        }

        [Test]
        public void Constructor_NullTransmissionRepository_DoesNotThrowButWillFailOnMethodCall()
        {
            // Arrange, Act & Assert - Constructor doesn't validate, but method calls will fail
            CarInfoService service = null;
            Assert.DoesNotThrow(() => service = new CarInfoService(
                null,
                _mockBrandRepository.Object,
                _mockCategoryRepository.Object,
                _mockFuelTypeRepository.Object,
                null));

            // Verify that calling the method with null repository throws
            Assert.ThrowsAsync<NullReferenceException>(async () => await service.GetTransmissionsAsync());
        }

        #endregion

        #region GetBrandsAsync Tests

        [Test]
        public async Task GetBrandsAsync_ValidData_ReturnsCorrectBrands()
        {
            // Arrange
            var brandId1 = Guid.NewGuid();
            var brandId2 = Guid.NewGuid();
            var brands = new List<Brand>
            {
                new Brand { Id = brandId1, Name = "Toyota" },
                new Brand { Id = brandId2, Name = "Honda" }
            }.AsQueryable();

            var mockQueryable = CreateMockQueryable(brands);
            _mockBrandRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetBrandsAsync();

            // Assert
            Assert.IsNotNull(result);
            var brandList = result.ToList();
            Assert.AreEqual(2, brandList.Count);
            
            Assert.AreEqual(brandId1, brandList[0].Id);
            Assert.AreEqual("Toyota", brandList[0].Name);
            
            Assert.AreEqual(brandId2, brandList[1].Id);
            Assert.AreEqual("Honda", brandList[1].Name);

            _mockBrandRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetBrandsAsync_EmptyData_ReturnsEmptyCollection()
        {
            // Arrange
            var brands = new List<Brand>().AsQueryable();
            var mockQueryable = CreateMockQueryable(brands);
            _mockBrandRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetBrandsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockBrandRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetBrandsAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockBrandRepository.Setup(r => r.GetAllAttached())
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _carInfoService.GetBrandsAsync());
            
            Assert.AreEqual("Database error", ex.Message);
        }

        #endregion

        #region GetCategoriesAsync Tests

        [Test]
        public async Task GetCategoriesAsync_ValidData_ReturnsCorrectCategories()
        {
            // Arrange
            var categoryId1 = Guid.NewGuid();
            var categoryId2 = Guid.NewGuid();
            var categories = new List<Category>
            {
                new Category { Id = categoryId1, Name = "SUV" },
                new Category { Id = categoryId2, Name = "Sedan" }
            }.AsQueryable();

            var mockQueryable = CreateMockQueryable(categories);
            _mockCategoryRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            var categoryList = result.ToList();
            Assert.AreEqual(2, categoryList.Count);
            
            Assert.AreEqual(categoryId1, categoryList[0].Id);
            Assert.AreEqual("SUV", categoryList[0].Name);
            
            Assert.AreEqual(categoryId2, categoryList[1].Id);
            Assert.AreEqual("Sedan", categoryList[1].Name);

            _mockCategoryRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetCategoriesAsync_EmptyData_ReturnsEmptyCollection()
        {
            // Arrange
            var categories = new List<Category>().AsQueryable();
            var mockQueryable = CreateMockQueryable(categories);
            _mockCategoryRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetCategoriesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockCategoryRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetCategoriesAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockCategoryRepository.Setup(r => r.GetAllAttached())
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _carInfoService.GetCategoriesAsync());
            
            Assert.AreEqual("Database error", ex.Message);
        }

        #endregion

        #region GetFuelTypesAsync Tests

        [Test]
        public async Task GetFuelTypesAsync_ValidData_ReturnsCorrectFuelTypes()
        {
            // Arrange
            var fuelTypeId1 = Guid.NewGuid();
            var fuelTypeId2 = Guid.NewGuid();
            var fuelTypes = new List<FuelType>
            {
                new FuelType { Id = fuelTypeId1, Type = "Gasoline" },
                new FuelType { Id = fuelTypeId2, Type = "Diesel" }
            }.AsQueryable();

            var mockQueryable = CreateMockQueryable(fuelTypes);
            _mockFuelTypeRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetFuelTypesAsync();

            // Assert
            Assert.IsNotNull(result);
            var fuelTypeList = result.ToList();
            Assert.AreEqual(2, fuelTypeList.Count);
            
            Assert.AreEqual(fuelTypeId1, fuelTypeList[0].Id);
            Assert.AreEqual("Gasoline", fuelTypeList[0].Type);
            
            Assert.AreEqual(fuelTypeId2, fuelTypeList[1].Id);
            Assert.AreEqual("Diesel", fuelTypeList[1].Type);

            _mockFuelTypeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetFuelTypesAsync_EmptyData_ReturnsEmptyCollection()
        {
            // Arrange
            var fuelTypes = new List<FuelType>().AsQueryable();
            var mockQueryable = CreateMockQueryable(fuelTypes);
            _mockFuelTypeRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetFuelTypesAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockFuelTypeRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetFuelTypesAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockFuelTypeRepository.Setup(r => r.GetAllAttached())
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _carInfoService.GetFuelTypesAsync());
            
            Assert.AreEqual("Database error", ex.Message);
        }

        #endregion

        #region GetTransmissionsAsync Tests

        [Test]
        public async Task GetTransmissionsAsync_ValidData_ReturnsCorrectTransmissions()
        {
            // Arrange
            var transmissionId1 = Guid.NewGuid();
            var transmissionId2 = Guid.NewGuid();
            var transmissions = new List<Transmission>
            {
                new Transmission { Id = transmissionId1, Type = "Manual" },
                new Transmission { Id = transmissionId2, Type = "Automatic" }
            }.AsQueryable();

            var mockQueryable = CreateMockQueryable(transmissions);
            _mockTransmissionRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetTransmissionsAsync();

            // Assert
            Assert.IsNotNull(result);
            var transmissionList = result.ToList();
            Assert.AreEqual(2, transmissionList.Count);
            
            Assert.AreEqual(transmissionId1, transmissionList[0].Id);
            Assert.AreEqual("Manual", transmissionList[0].Type);
            
            Assert.AreEqual(transmissionId2, transmissionList[1].Id);
            Assert.AreEqual("Automatic", transmissionList[1].Type);

            _mockTransmissionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public async Task GetTransmissionsAsync_EmptyData_ReturnsEmptyCollection()
        {
            // Arrange
            var transmissions = new List<Transmission>().AsQueryable();
            var mockQueryable = CreateMockQueryable(transmissions);
            _mockTransmissionRepository.Setup(r => r.GetAllAttached()).Returns(mockQueryable);

            // Act
            var result = await _carInfoService.GetTransmissionsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockTransmissionRepository.Verify(r => r.GetAllAttached(), Times.Once);
        }

        [Test]
        public void GetTransmissionsAsync_RepositoryThrowsException_PropagatesException()
        {
            // Arrange
            _mockTransmissionRepository.Setup(r => r.GetAllAttached())
                .Throws(new InvalidOperationException("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _carInfoService.GetTransmissionsAsync());
            
            Assert.AreEqual("Database error", ex.Message);
        }

        #endregion

        #region Integration Tests

        [Test]
        public async Task GetAllDataAsync_AllMethodsWork_ReturnsValidData()
        {
            // Arrange
            SetupAllRepositories();

            // Act
            var brands = await _carInfoService.GetBrandsAsync();
            var categories = await _carInfoService.GetCategoriesAsync();
            var fuelTypes = await _carInfoService.GetFuelTypesAsync();
            var transmissions = await _carInfoService.GetTransmissionsAsync();

            // Assert
            Assert.IsNotNull(brands);
            Assert.IsNotNull(categories);
            Assert.IsNotNull(fuelTypes);
            Assert.IsNotNull(transmissions);

            Assert.AreEqual(1, brands.Count());
            Assert.AreEqual(1, categories.Count());
            Assert.AreEqual(1, fuelTypes.Count());
            Assert.AreEqual(1, transmissions.Count());
        }

        #endregion

        #region Helper Methods

        private IQueryable<T> CreateMockQueryable<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return mockSet.Object;
        }

        private void SetupAllRepositories()
        {
            var brands = new List<Brand>
            {
                new Brand { Id = Guid.NewGuid(), Name = "Test Brand" }
            }.AsQueryable();

            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Test Category" }
            }.AsQueryable();

            var fuelTypes = new List<FuelType>
            {
                new FuelType { Id = Guid.NewGuid(), Type = "Test Fuel" }
            }.AsQueryable();

            var transmissions = new List<Transmission>
            {
                new Transmission { Id = Guid.NewGuid(), Type = "Test Transmission" }
            }.AsQueryable();

            _mockBrandRepository.Setup(r => r.GetAllAttached()).Returns(CreateMockQueryable(brands));
            _mockCategoryRepository.Setup(r => r.GetAllAttached()).Returns(CreateMockQueryable(categories));
            _mockFuelTypeRepository.Setup(r => r.GetAllAttached()).Returns(CreateMockQueryable(fuelTypes));
            _mockTransmissionRepository.Setup(r => r.GetAllAttached()).Returns(CreateMockQueryable(transmissions));
        }

        #endregion
    }

    #region Test Helper Classes for Async Testing

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var result = Execute<TResult>(expression);
            return result;
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public T Current => _inner.Current;
    }

    #endregion
}