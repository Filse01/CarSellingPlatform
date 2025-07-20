namespace CarSellingPlatform.Web.ViewModels.Chat;

public class IndexChatViewModel
{
    public Guid Id { get; set; }
    public string User { get; set; } = null!;
    public string Seller { get; set; } = null!;
    public string CarModel { get; set; } = null!;
    public string CarBrand { get; set; } = null!;
}