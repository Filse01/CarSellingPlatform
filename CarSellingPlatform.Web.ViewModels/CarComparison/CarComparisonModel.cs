namespace CarSellingPlatform.Web.ViewModels.CarComparison;
using CarSellingPlatform.Data.Models.Car;
public class CarComparisonModel
{
    public IEnumerable<Car> AvailableCars { get; set; } = new List<Car>();
    public Car? Car1 { get; set; }
    public Car? Car2 { get; set; }
    public string? SearchTerm1 { get; set; }
    public string? SearchTerm2 { get; set; }
}