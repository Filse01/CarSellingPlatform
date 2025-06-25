using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models;

public class FuelType
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Type { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}