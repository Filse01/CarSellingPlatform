using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Data.Models.Forum;

public class Post
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(80)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string AuthorId { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = null!;
}