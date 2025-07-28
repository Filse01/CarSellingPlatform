namespace CarSellingPlatform.Web.ViewModels.Car;

public class MyCarsViewModel
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public decimal Price { get; set; }
    public byte[]? ImageData { get; set; }
}