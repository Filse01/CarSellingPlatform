using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using static CarSellingPlatform.Data.Common.ApplicationUser;
namespace CarSellingPlatform.Data.Models.Chat;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(FirstNameMaxLenght)]
    public string FirstName { get; set; } = null!;
    [Required]
    [MaxLength(LastNameMaxLenght)]
    public string LastName { get; set; } = null!;
}