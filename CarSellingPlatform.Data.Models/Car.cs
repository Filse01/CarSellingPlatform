using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models;

public class Car
{
    [Key]
    public int Id { get; set; }

    [Required] 
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    
    public string? ImageUrl { get; set; }
    [Required]
    public decimal Price { get; set; }
    
    public int Year { get; set; }
    
    public string Color { get; set; } = null!;
    
    public int CategoryId { get; set; }
    
    public Category Category { get; set; } = null!;
    
    public int EngineId { get; set; }
    
    public Engine Engine { get; set; } = null!;
    
    public int TransmissionId { get; set; }
    
    public Transmission Transmission { get; set; } = null!;
    
    public int FuelTypeId { get; set; }
    
    public FuelType FuelType { get; set; } = null!;
    
    public int UserId { get; set; }
}