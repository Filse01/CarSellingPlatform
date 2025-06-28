using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.Car;
namespace CarSellingPlatform.Data.Models;

public class Brand
{
    public int Id { get; set; }
    [Required]
    [StringLength(BrandMaxLength, MinimumLength = BrandMinLength)]
    public string Name { get; set; } = null!;
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}