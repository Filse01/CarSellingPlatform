using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static CarSellingPlatform.Data.Common.Message;
namespace CarSellingPlatform.Data.Models.Chat;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required] 
    [MaxLength(TextMaxLength)]
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required]
    public string CreatorId { get; set; }
    [ForeignKey("CreatorId")]
    public ApplicationUser Creator { get; set; } = null!;
    public Guid ChatId { get; set; }
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
}