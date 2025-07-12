using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;
using Microsoft.AspNetCore.Identity;

namespace CarSellingPlatform.Data.Models.Car;
using static CarSellingPlatform.Data.Common.Car;
public class Car
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] 
    [StringLength(BrandMaxLength, MinimumLength = BrandMinLength)]
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; }
    [Required]
    [StringLength(ModelMaxLength, MinimumLength = ModelMinLength)]
    public string Model { get; set; } = null!;
    [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    public string Color { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
    [Required]
    public string SellerId { get; set; } = null!;
    public ApplicationUser Seller { get; set; } = null!;
    public Guid CategoryId { get; set; }
    
    public Category Category { get; set; } = null!;
    
    public Guid EngineId { get; set; }
    
    public Engine Engine { get; set; } = null!;
    
    public Guid TransmissionId { get; set; }
    
    public Transmission Transmission { get; set; } = null!;
    
    public Guid FuelTypeId { get; set; }
    
    public FuelType FuelType { get; set; } = null!;
    
    public ICollection<UserCar> UserCars { get; set; } = new HashSet<UserCar>();
    
    public ICollection<Chat.Chat> Chats { get; set; } = new HashSet<Chat.Chat>();
}