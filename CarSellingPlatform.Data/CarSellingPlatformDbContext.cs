namespace CarSellingPlatform.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class CarSellingPlatformDbContext : IdentityDbContext
    {
        public CarSellingPlatformDbContext(DbContextOptions<CarSellingPlatformDbContext> options)
            : base(options)
        {

        }
    }
}
