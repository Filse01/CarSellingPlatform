using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Web.ViewModels.Dealership;

public class AddDealershipInputModel
{
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
    
}