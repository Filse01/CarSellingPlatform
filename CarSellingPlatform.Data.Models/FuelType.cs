namespace CarSellingPlatform.Data.Models;

public class FuelType
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}