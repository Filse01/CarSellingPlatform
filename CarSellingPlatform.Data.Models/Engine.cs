using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Data.Models;

public class Engine
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string Displacement { get; set; } = null!;
    [Required]
    public int Horsepower { get; set; }
    [Required]
    public int Cylinders { get; set; }
    public string? EngineCode {get; set;}
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}