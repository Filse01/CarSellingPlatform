using CarSellingPlatform.Data.Interfaces.Repository;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Services.Core.Contracts;
using CarSellingPlatform.Web.ViewModels.Car;
using CarSellingPlatform.Web.ViewModels.CarManagement;
using CarSellingPlatform.Web.ViewModels.Dealership;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarSellingPlatform.Services.Core;

public class DealerShipService : IDealerShipService
{
    private readonly IRepository<Dealership,Guid> _dealershipRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepository<Car, Guid> _carRepository;
    public DealerShipService(IRepository<Dealership, Guid> dealershipRepository, UserManager<ApplicationUser> userManager, IRepository<Car, Guid> carRepository)
    {
        _dealershipRepository = dealershipRepository;
        _userManager = userManager;
        _carRepository = carRepository;
    }
    public async Task<PagedListViewModel<IndexCarViewModel>> ListPagedAsync(Guid id,string? userId, int pageNumber, int pageSize)
    {
        var query = _carRepository.GetAllAttached()
            .Where(c => c.DealershipId == id)
            .Include(c => c.Category)
            .Include(c => c.FuelType)
            .Include(c => c.Brand)
            .Include(c => c.Engine)
            .Include(c => c.Transmission)
            .AsNoTracking();

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
    public async Task<PagedListViewModel<IndexDealershipViewModel>> ListPagedAsync(string? userId, int pageNumber, int pageSize)
    {
        var query = _dealershipRepository.GetAllAttached()
            .AsNoTracking();
        int totalCount = await query.CountAsync();
        var dealerships = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new IndexDealershipViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                PhoneNumber = c.PhoneNumber,
                Logo = c.Logo,
                Description = c.Description,
            })
            .ToListAsync();

        return new PagedListViewModel<IndexDealershipViewModel>
        {
            Items = dealerships,
            PageNumber = pageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
    public async Task<bool> AddDealershipAsync(string userId, AddDealershipInputModel model, IFormFile? imageFile)
    {
        bool opResult = false;
        
        var user = await _userManager.FindByIdAsync(userId);
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
           
            Dealership newDealership = new Dealership()
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Address = model.Address,
                Logo = imageData,
                Description = model.Description,
                PhoneNumber = model.PhoneNumber,
                OwnerId = userId,
                };
            await _dealershipRepository.AddAsync(newDealership);
            await _dealershipRepository.SaveChangesAsync();
            opResult = true;
        }
        return opResult;
    }

    public async Task<DetailsDealershipViewModel> GetDetailsDealershipAsync(Guid? id, string userId)
    {
        DetailsDealershipViewModel? model = null;
        if (id.HasValue)
        {
            Dealership dealershipModel = await _dealershipRepository
                .GetAllAttached()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id.Value);
            if (dealershipModel != null)
            {
                model = new DetailsDealershipViewModel()
                {
                    Id = dealershipModel.Id,
                    Name = dealershipModel.Name,
                    Address = dealershipModel.Address,
                    PhoneNumber = dealershipModel.PhoneNumber,
                    Description = dealershipModel.Description,
                    Logo = dealershipModel.Logo,
                    };
            }
        }
        return model;
    }

    public async Task<(byte[] ImageData, string ContentType)?> GetDealershipImageByIdAsync(Guid id)
    {
        var dealership = await _dealershipRepository.FirstOrDefaultAsync(c => c.Id == id);
        if (dealership != null && dealership.Logo != null)
        {
            return (dealership.Logo, "image/jpeg"); 
        }

        return null;
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
                .Include(c =>c.Dealership)
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
                    DealershipId = carModel.DealershipId
                };
            }
        }
        return model;
    }
}