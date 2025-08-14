using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;
using static CarSellingPlatform.Data.Common.Post;
namespace CarSellingPlatform.Data.Models.Forum;

public class Post
{
    public Guid Id { get; set; }
    [Required]
    [MaxLength(TextMaxLength)]
    public string Title { get; set; } = null!;
    [Required]
    [MaxLength(TextMaxLength)]
    public string Text { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string AuthorId { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = null!;
}