namespace CarSellingPlatform.Data.Common;

public static class Car
{
    public const int BrandMaxLength = 30;
    public const int BrandMinLength = 1;
    
    public const int DescriptionMaxLength = 300;
    public const int DescriptionMinLength = 10;
    
    public const int ModelMaxLength = 30;
    public const int ModelMinLength = 1;
    
    public const int ColorMaxLength = 30;
    
    public const int ImageUrlMaxLength = 2083;
    
    public const int HorsePowerMinLength = 1;
    public const int HorsePowerMaxLength = 2000;
    
    public const int PriceMinLength = 1;
    public const int PriceMaxLength = 10_000_000;
    
    public const int YearMinLength = 1886;
    public const int YearMaxLength = 2100;
}