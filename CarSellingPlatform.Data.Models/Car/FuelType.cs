using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.FuelType;
namespace CarSellingPlatform.Data.Models.Car;

public class FuelType
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(20)]
    public string Type { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}