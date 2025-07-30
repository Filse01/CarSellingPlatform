using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Car;
public class Chat
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = null!;
    [Required]
    public string SellerId { get; set; }
    [ForeignKey(nameof(SellerId))]
    public ApplicationUser Seller { get; set; } = null!;
    public Guid CarId { get; set; }
    [ForeignKey(nameof(CarId))]
    public Car Car { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}