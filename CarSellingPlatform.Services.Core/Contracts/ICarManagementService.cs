using CarSellingPlatform.Web.ViewModels.CarManagement;
using CarSellingPlatform.Web.ViewModels.UserManager;

namespace CarSellingPlatform.Services.Core.Contracts;

public interface ICarManagementService
{
    Task<IEnumerable<CarManagementIndexView>> GetAllUCars();
}