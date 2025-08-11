using CarSellingPlatform.Web.ViewModels.Dealership;

namespace CarSellingPlatform.Web.ViewModels.Car;

public class DetailsCarViewModel : IndexCarViewModel
{
    public int Cylinders { get; set; }
    
    public string PhoneNubmer { get; set; }
    
    public Guid? DealershipId { get; set; }
    public DetailsDealershipViewModel Dealership{get;set;}
}