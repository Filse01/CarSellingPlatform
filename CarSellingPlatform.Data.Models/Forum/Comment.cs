using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Data.Models.Forum;

public class Comment
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(300)]
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string AuthorId { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public Guid PostId { get; set; }
    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = null!;
}