using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Name { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}