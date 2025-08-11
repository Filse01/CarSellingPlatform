namespace CarSellingPlatform.Web.ViewModels.Dealership;

public class IndexDealershipViewModel
{
  
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public string PhoneNumber { get; set; } = null!;

    public byte[]? Logo { get; set; }

    public string? Description { get; set; }
}