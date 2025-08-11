using System.ComponentModel.DataAnnotations;

namespace CarSellingPlatform.Web.ViewModels.Forum;

public class AddPostInputModel
{
    [Required]
    [MaxLength(80)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = null!;
}