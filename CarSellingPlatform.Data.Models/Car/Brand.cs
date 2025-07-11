using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.Car;
namespace CarSellingPlatform.Data.Models.Car;

public class Brand
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [StringLength(BrandMaxLength, MinimumLength = BrandMinLength)]
    public string Name { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}