using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;
using CarSellingPlatform.Data.Models.Forum;

namespace CarSellingPlatform.Web.ViewModels.Forum;

public class DetailsPostViewModel
{
    public Post Post { get; set; } = null!;

    [Required(ErrorMessage = "Comment cannot be empty.")]
    [Display(Name = "Your Comment")]
    public string NewCommentText { get; set; } = null!;
    public ApplicationUser Author { get; set; } = null!;
    public bool IsOwner { get; set; }
}