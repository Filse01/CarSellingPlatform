using CarSellingPlatform.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarSellingPlatform.Data.Configuration;

public class UserCarConfiguration : IEntityTypeConfiguration<UserCar>
{
    public void Configure(EntityTypeBuilder<UserCar> builder)
    {
        builder
            .HasKey(uc => new { uc.UserId, uc.CarId });
        
        builder
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(uc => uc.Car)
            .WithMany(uc => uc.UserCars)
            .HasForeignKey(uc => uc.CarId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasQueryFilter(c => c.Car.IsDeleted == false);
    }
}