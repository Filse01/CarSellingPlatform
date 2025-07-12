using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarSellingPlatform.Data.Models.Chat;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required] 
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string CreatorId { get; set; }
    [ForeignKey("CreatorId")]
    public ApplicationUser Creator { get; set; } = null!;
    public Guid ChatId { get; set; }
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
}