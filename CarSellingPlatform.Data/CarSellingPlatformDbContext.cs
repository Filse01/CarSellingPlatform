using System.Reflection;
using CarSellingPlatform.Data.Models.Car;
using CarSellingPlatform.Data.Models.Chat;

namespace CarSellingPlatform.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class CarSellingPlatformDbContext : IdentityDbContext<ApplicationUser>
    {
        public CarSellingPlatformDbContext(DbContextOptions<CarSellingPlatformDbContext> options)
            : base(options)
        {

        }
        
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<Engine> Engines { get; set; }
        public DbSet<Transmission> Transmissions { get; set; }
        public DbSet<Brand> Brands { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
