using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;

namespace CarSellingPlatform.Web.ViewModels.Forum;

public class IndexPostViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}