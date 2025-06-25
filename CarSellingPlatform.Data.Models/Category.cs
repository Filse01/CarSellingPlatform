using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}