namespace CarSellingPlatform.Web.ViewModels.Car;

public class PagedListViewModel<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IEnumerable<AddCarBrand> Brands { get; set; } = new List<AddCarBrand>();
    public IEnumerable<AddCarTransmission> Transmissions { get; set; } = new List<AddCarTransmission>();
    public IEnumerable<AddCarCategory> Categories { get; set; } = new List<AddCarCategory>();
}