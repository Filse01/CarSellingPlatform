using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models.Car;

public class FuelType
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Type { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}