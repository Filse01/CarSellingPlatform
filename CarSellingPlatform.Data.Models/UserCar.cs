using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CarSellingPlatform.Data.Models;

public class UserCar
{
    [Required] 
    public string UserId { get; set; } = null!;
    public IdentityUser User { get; set; }
    [Required]
    public int CarId { get; set; }
    public Car Car { get; set; }
}