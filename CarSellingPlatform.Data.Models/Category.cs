namespace CarSellingPlatform.Data.Models;

public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}