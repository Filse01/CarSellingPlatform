using System.ComponentModel.DataAnnotations;
using static CarSellingPlatform.Data.Common.Engine;
namespace CarSellingPlatform.Data.Models.Car;

public class Engine
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    [MaxLength(DisplacementMaxLength)]
    public string Displacement { get; set; } = null!;
    [Required]
    public int Horsepower { get; set; }
    [Required]
    public int Cylinders { get; set; }
    [MaxLength(EngineCodeMaxLength)]
    public string? EngineCode {get; set;}
    
    public ICollection<Car> Cars { get; set; } = new HashSet<Car>();
}