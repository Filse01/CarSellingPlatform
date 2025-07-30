using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.Category;
namespace CarSellingPlatform.Data.Models.Car;

public class Category
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(NameMaxLength)]
    public string Name { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}