namespace CarSellingPlatform.Web.ViewModels.Car;

public class IndexCarViewModel
{
    public Guid Id { get; set; }
    
    public string? ImageUrl { get; set; }
    public string BrandName { get; set; }
    
    public Guid BrandId { get; set; }
    public string CarModel { get; set; } = null!;
    
    public string Description { get; set; } = null!;
    
    public string CategoryName { get; set; }
    
    public string FuelTypeName { get; set; }
    
    public string TransmissionTypeName { get; set; }
    
    public int HorsePower { get; set; }
    
    public string Color { get; set; } = null!;
    
    public string Displacement { get; set; } = null!;
    
    public int Year { get; set; }
    
    public decimal Price { get; set; }
    
    public bool IsUserSeller { get; set; }
    public bool IsUserFavorite { get; set; }
    public string SellerId { get; set; } = null!;
    public IEnumerable<AddCarBrand>? Brands { get; set; }
}