using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Data.Models.Car;

public class Dealership
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Address { get; set; } = null!;

    [Required]
    [MaxLength(12)]
    public string PhoneNumber { get; set; } = null!;

    public byte[]? Logo { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public string OwnerId { get; set; } = null!;
    public ApplicationUser Owner { get; set; } = null!;

    public ICollection<Car> Cars { get; set; } = new List<Car>();
}