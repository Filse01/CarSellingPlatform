using CarSellingPlatform.Data;
using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Repository;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CarSellingPlatform.Services.Core;

public class CarService : ICarService
{
    private readonly IRepository<Car,Guid> _carRepository;
    private readonly IRepository<Engine, Guid> _engineRepository;
    private readonly IRepository<UserCar, Guid> _userCarRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public CarService(IRepository<Car, Guid> carRepository, IRepository<Engine, Guid> engineRepository,IRepository<UserCar, Guid> userCarRepository ,UserManager<ApplicationUser> userManager)
    {
        _carRepository = carRepository;
        _engineRepository = engineRepository;
        _userManager = userManager;
        _userCarRepository = userCarRepository;
    }

    public async Task<PagedListViewModel<IndexCarViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize, string? search = null, Guid? brandId = null)
    {
        var query = _carRepository.GetAllAttached()
            .Include(c => c.Category)
            .Include(c => c.FuelType)
            .Include(c => c.Brand)
            .Include(c => c.Engine)
            .Include(c => c.Transmission)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            string loweredSearch = search.ToLower();
            query = query.Where(c =>
                c.Model.ToLower().Contains(loweredSearch) ||
                c.Brand.Name.ToLower().Contains(loweredSearch));
        }
        if (brandId.HasValue)
        {
            query = query.Where(c => c.BrandId == brandId);
        }
        int totalCount = await query.CountAsync();

        var cars = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new IndexCarViewModel
            {
                Id = c.Id,
                BrandId = c.Brand.Id,
                BrandName = c.Brand.Name,
                CategoryName = c.Category.Name,
                CarModel = c.Model,
                Description = c.Description,
                HorsePower = c.Engine.Horsepower,
                Color = c.Color,
                TransmissionTypeName = c.Transmission.Type,
                Year = c.Year,
                Displacement = c.Engine.Displacement,
                Price = c.Price,
                FuelTypeName = c.FuelType.Type,
                ImageUrl = c.ImageUrl,
                IsUserSeller = userId != null ?
                    c.SellerId.ToLower() == userId.ToLower() : false,
                SellerId = c.SellerId,
                ImageData = c.ImageData,
                IsUserFavorite = userId != null ?
                    c.UserCars.Any(c => c.UserId.ToLower() == userId.ToLower()) : false,
            })
            .ToListAsync();

        return new PagedListViewModel<IndexCarViewModel>
        {
            Items = cars,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<bool> AddCarAsync(string userId, AddCarViewModel model, IFormFile? imageFile)
    {
        bool opResult = false;
        
        var user = _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            byte[] imageData = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }
            Engine newEngine = new Engine()
            {
                Id = Guid.NewGuid(),
                Cylinders = model.Cylinders,
                Displacement = model.Displacement,
                Horsepower = model.Horsepower,
                EngineCode = model.EngineCode,
            };
            await _engineRepository.AddAsync(newEngine);
            Car newCar = new Car()
            {
                BrandId = model.BrandId,
                Model = model.CarModel,
                ImageUrl = model.ImageUrl,
                Description = model.Description,
                Price = model.Price,
                Color = model.Color,
                Year = model.Year,
                CategoryId = model.CategoryId,
                SellerId = userId,
                EngineId = newEngine.Id,
                TransmissionId = model.TransmissionId,
                FuelTypeId = model.FuelTypeId,
                ImageData = imageData
            };
            await _carRepository.AddAsync(newCar);
            await _carRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }

    public async Task<DetailsCarViewModel> GetDetailsCarAsync(Guid? id, string userId)
    {
        DetailsCarViewModel? model = null;
        if (id.HasValue)
        {
            Car carModel = await _carRepository
                .GetAllAttached()
                .Include(c => c.Category)
                .Include(c => c.FuelType)
                .Include(c => c.Brand)
                .Include(c => c.Engine)
                .Include(c => c.Transmission)
                .Include(c => c.Seller)
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            if (carModel != null)
            {
                model = new DetailsCarViewModel()
                {
                    Id = carModel.Id,
                    Cylinders = carModel.Engine.Cylinders,
                    BrandName = carModel.Brand.Name,
                    CategoryName = carModel.Category.Name,
                    CarModel = carModel.Model,
                    Description = carModel.Description,
                    HorsePower = carModel.Engine.Horsepower,
                    Color = carModel.Color,
                    TransmissionTypeName = carModel.Transmission.Type,
                    Year = carModel.Year,
                    ImageUrl = carModel.ImageUrl,
                    Displacement = carModel.Engine.Displacement,
                    Price = carModel.Price,
                    FuelTypeName = carModel.FuelType.Type,
                    IsUserSeller = userId != null ? carModel.SellerId.ToLower() == userId.ToLower() : false,
                    PhoneNubmer = carModel.Seller.PhoneNumber,
                    ImageData = carModel.ImageData,
                };
            }
        }
        return model;
    }

    public async Task<EditCarViewModel> GetEditCarAsync(Guid? id, string userId)
    {
        EditCarViewModel model = null;
        Engine oldEngine = null;
        if (id.HasValue)
        {
            Car? editCar = await _carRepository.GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            Engine editEngine = await _engineRepository.GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == editCar.EngineId);
            if (editCar != null && editCar.SellerId.ToLower() == userId.ToLower())
            {
                oldEngine = new Engine()
                {
                    Id = editEngine.Id,
                    Displacement = editEngine.Displacement,
                    Cylinders = editEngine.Cylinders,
                    Horsepower = editEngine.Horsepower,
                    EngineCode = editEngine.EngineCode,
                };
                model = new EditCarViewModel()
                {
                    Id = editCar.Id,
                    BrandId = editCar.BrandId,
                    CarModel = editCar.Model,
                    Description = editCar.Description,
                    Price = editCar.Price,
                    Color = editCar.Color,
                    TransmissionId = editCar.TransmissionId,
                    FuelTypeId = editCar.FuelTypeId,
                    Year = editCar.Year,
                    Displacement = oldEngine.Displacement,
                    Horsepower = oldEngine.Horsepower,
                    EngineCode = oldEngine.EngineCode,
                    ImageUrl = editCar.ImageUrl,
                    SellerId = userId,
                    Cylinders = oldEngine.Cylinders,
                    CategoryId = editCar.CategoryId,
                    EngineId = oldEngine.Id,
                    ImageData = editCar.ImageData,
                };
            }
            
        }
        return model;
    }

    public async Task<bool> EditCarAsync(string userId,EditCarViewModel model, IFormFile? imageFile)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);

        Car updatedCar = await _carRepository.GetAllAttached()
            .SingleOrDefaultAsync(c => c.Id == model.Id);
        Engine updatedEngine = await _engineRepository.GetAllAttached()
            .SingleOrDefaultAsync(c => c.Id == updatedCar.EngineId);
        if (updatedCar != null && user != null)
        {
            byte[] imageData = null;

            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            if (updatedCar.ImageData != null && imageData != null)
            {
                updatedCar.ImageData = imageData;
            }
            updatedCar.Model = model.CarModel;
            updatedCar.ImageUrl = model.ImageUrl;
            updatedCar.Description = model.Description;
            updatedCar.Price = model.Price;
            updatedCar.Color = model.Color;
            updatedCar.TransmissionId = model.TransmissionId;
            updatedCar.FuelTypeId = model.FuelTypeId;
            updatedCar.Year = model.Year;
            updatedCar.BrandId = model.BrandId;
            updatedCar.CategoryId = model.CategoryId;
            updatedEngine.Cylinders = model.Cylinders;
            updatedEngine.Horsepower = model.Horsepower;
            updatedEngine.EngineCode = model.EngineCode;
            await _carRepository.UpdateAsync(updatedCar);
            await _engineRepository.UpdateAsync(updatedEngine);
            await _carRepository.SaveChangesAsync();
            await _engineRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }

    public async Task<DeleteCarViewModel> GetDeleteCarAsync(Guid? id, string userId)
    {
        DeleteCarViewModel model = null;
        if (id.HasValue)
        {
            Car? deleteCar = _carRepository.GetAllAttached()
                .Include(c => c.Seller)
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == id.Value);
            if (deleteCar != null && deleteCar.SellerId.ToLower() == userId.ToLower())
            {
                model = new DeleteCarViewModel()
                {
                    Id = deleteCar.Id,
                    CarModel = deleteCar.Model,
                    Seller = deleteCar.Seller.UserName,
                    SellerId = userId,
                };
            }
        }
        return model;
    }

    public async Task<bool> SoftDeleteCarAsync(DeleteCarViewModel model, string userId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        Car deletedCar = await _carRepository.SingleOrDefaultAsync(c => c.Id == model.Id);
        if (user != null && deletedCar != null)
        {
            deletedCar.IsDeleted = true;
            await _carRepository.UpdateAsync(deletedCar);
            await _carRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }

    public async Task<PagedListViewModel<FavoriteCarViewModel>?> GetFavoriteCarsAsync(string userId, int pageNumber, int pageSize)
    {
        IEnumerable<FavoriteCarViewModel>? favoriteCars = null;
        var user = await _userManager.FindByIdAsync(userId);
        var query =  _userCarRepository.GetAllAttached()
            .Include(c => c.Car)
            .AsNoTracking();
        int totalCount = await query.CountAsync();
        if (user != null)
        {
            
            favoriteCars = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new FavoriteCarViewModel()
                {
                    Id = c.Car.Id,
                    BrandName = c.Car.Brand.Name,
                    CarModel = c.Car.Model,
                    ImageUrl = c.Car.ImageUrl,
                    Description = c.Car.Description,
                    Price = c.Car.Price,
                    CategoryName = c.Car.Category.Name,
                    FuelTypeName = c.Car.FuelType.Type,
                    TransmissionTypeName = c.Car.Transmission.Type,
                    HorsePower = c.Car.Engine.Horsepower,
                    Color = c.Car.Color,
                    Displacement = c.Car.Engine.Displacement,
                    Year = c.Car.Year,
                    SellerId = c.Car.SellerId,
                }).ToListAsync();
        }
        return new PagedListViewModel<FavoriteCarViewModel>
        {
            Items = favoriteCars,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<bool> AddCarToFavoritesAsync(string userId, Guid carId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        Car favCar = await _carRepository.GetByIdAsync(carId);
        if (user != null && favCar != null && favCar.SellerId.ToLower() != userId.ToLower())
        {
            UserCar? userCar = await _userCarRepository.SingleOrDefaultAsync(c => c.UserId.ToLower() == userId.ToLower() && c.CarId == favCar.Id);
            if (userCar == null)
            {
                userCar = new UserCar()
                {
                    UserId = userId,
                    CarId = carId
                };
                await _userCarRepository.AddAsync(userCar);
                await _userCarRepository.SaveChangesAsync();
                opResult = true;
            }
        }
        return opResult;
    }

    public async Task<bool> RemoveCarFromFavoritesAsync(string userId, Guid carId)
    {
        bool opResult = false;
        var user = await _userManager.FindByIdAsync(userId);
        Car deletedCar = await _carRepository.SingleOrDefaultAsync(c => c.Id == carId);
        if (user != null && deletedCar != null)
        {
            UserCar? userCar = await _userCarRepository.SingleOrDefaultAsync(c => c.UserId.ToLower() == userId.ToLower()
            && c.CarId == carId);
            if (userCar != null)
            {
                _userCarRepository.HardDelete(userCar);
                await _userCarRepository.SaveChangesAsync();
                opResult = true;
            }
        }
        return opResult;
    }

    public async Task<PagedListViewModel<MyCarsViewModel>> MyCarsPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _carRepository.GetAllAttached()
            .Include(c => c.Brand)
            .Where(c => c.SellerId == userId)
            .AsNoTracking();
        int totalCount = await query.CountAsync();

        var cars = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new MyCarsViewModel
            {
                Id = c.Id,
                ImageUrl = c.ImageUrl,
                Brand = c.Brand.Name,
                CarModel = c.Model,
                Price = c.Price,
                ImageData = c.ImageData
            })
            .ToListAsync();

        return new PagedListViewModel<MyCarsViewModel>
        {
            Items = cars,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}