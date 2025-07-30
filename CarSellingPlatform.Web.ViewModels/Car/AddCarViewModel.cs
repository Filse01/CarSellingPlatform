using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.GCommon;
using static CarSellingPlatform.Data.Common.Engine;
using static CarSellingPlatform.Data.Common.Car;

namespace CarSellingPlatform.Web.ViewModels.Car;

public class AddCarViewModel
{
    [Required]
    public Guid BrandId { get; set; }

    [Required]
    [StringLength(ModelMaxLength, MinimumLength = ModelMinLength)]
    public string CarModel { get; set; } = null!;

    [Required]
    [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
    public string? Description { get; set; } = null!;
    
    [Display(Name = "Image URL")]
    [MaxLength(ImageUrlMaxLength)]
    public string? ImageUrl { get; set; } 

    [Required]
    [Range(PriceMinLength, PriceMaxLength)]
    public decimal Price { get; set; }

    [Required]
    [Range(YearMinLength, YearMaxLength)]
    public int Year { get; set; }

    [Required]
    [MaxLength(ColorMaxLength)]
    public string Color { get; set; } = null!;

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [RegularExpression(@"^\d+\.\d+[Ll]?$", ErrorMessage = "Please enter a valid displacement (e.g., 2.0L")]
    public string Displacement { get; set; } = null!;

    [Required]
    [Range(HorsePowerMinLength, HorsePowerMaxLength)]
    public int Horsepower { get; set; }

    [Required(ErrorMessage = ErrorMessages.Cylinders)]
    [Range(CylinderMinLength, CylinderMaxLength)]
    public int Cylinders { get; set; }
    [MaxLength(EngineCodeMaxLength)]
    public string? EngineCode { get; set; }
    
    public byte[]? ImageData { get; set; }

    [Required]
    public Guid FuelTypeId { get; set; }

    [Required]
    public Guid TransmissionId { get; set; } 

    public IEnumerable<AddCarBrand>? Brands { get; set; }
    public IEnumerable<AddCarCategory>? Categories { get; set; }
    public IEnumerable<AddCarFuelType>? FuelTypes { get; set; } 
    public IEnumerable<AddCarTransmission>? Transmissions { get; set; } 
}