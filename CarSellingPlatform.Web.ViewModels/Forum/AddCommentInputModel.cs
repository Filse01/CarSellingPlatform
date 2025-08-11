using System.ComponentModel.DataAnnotations;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Web.ViewModels.Forum;

public class AddCommentInputModel
{
    [Required]
    [MaxLength(300)]
    public string NewCommentText { get; set; } = null!;
}