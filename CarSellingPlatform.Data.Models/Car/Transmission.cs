using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.Transmission;
namespace CarSellingPlatform.Data.Models.Car;

public class Transmission
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(TypeMaxLength)]
    public string Type { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}