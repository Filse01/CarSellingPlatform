namespace CarSellingPlatform.Web.ViewModels.Car;

public class EditCarViewModel : AddCarViewModel
{ 
    public Guid Id { get; set; }
    public string SellerId { get; set; } = null!;
    public Guid EngineId { get; set; }
}