using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;
using Microsoft.AspNetCore.Identity;

namespace CarSellingPlatform.Data.Models.Car;

public class UserCar
{
    [Required] 
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; }
    [Required]
    public Guid CarId { get; set; }
    public Car Car { get; set; }
}