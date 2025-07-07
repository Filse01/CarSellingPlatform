namespace CarSellingPlatform.Web.ViewModels.Car;

public class DeleteCarViewModel
{
    public Guid Id { get; set; }
    
    public string CarModel { get; set; }
    
    public string? Seller { get; set; }
    
    public string SellerId { get; set; }
}