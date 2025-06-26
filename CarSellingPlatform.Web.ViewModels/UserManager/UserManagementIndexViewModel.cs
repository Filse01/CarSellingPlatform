namespace CarSellingPlatform.Web.ViewModels.UserManager;

public class UserManagementIndexViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public bool EmailConfirmed { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}