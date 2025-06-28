using CarSellingPlatform.Web.ViewModels.Car;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarInfoService
{
    Task<IEnumerable<AddCarBrand>> GetBrandsAsync();
    Task<IEnumerable<AddCarCategory>> GetCategoriesAsync();
    Task<IEnumerable<AddCarFuelType>> GetFuelTypesAsync();
    Task<IEnumerable<AddCarTransmission>> GetTransmissionsAsync();
}