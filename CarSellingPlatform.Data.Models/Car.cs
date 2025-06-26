using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarSellingPlatform.Data.Models;
using static CarSellingPlatform.Data.Common.Car;
public class Car
{
    [Key]
    public int Id { get; set; }

    [Required] 
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
    public string Title { get; set; } = null!;
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
    public IdentityUser Seller { get; set; } = null!;
    public int CategoryId { get; set; }
    
    public Category Category { get; set; } = null!;
    
    public int EngineId { get; set; }
    
    public Engine Engine { get; set; } = null!;
    
    public int TransmissionId { get; set; }
    
    public Transmission Transmission { get; set; } = null!;
    
    public int FuelTypeId { get; set; }
    
    public FuelType FuelType { get; set; } = null!;
    
    public ICollection<UserCar> UserCars { get; set; } = new HashSet<UserCar>();
}