using CarSellingPlatform.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarSellingPlatform.Data.Configuration;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder
            .HasOne(c =>c.Engine)
            .WithMany(e => e.Cars)
            .HasForeignKey(c => c.EngineId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Category)
            .WithMany(e => e.Cars)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Transmission)
            .WithMany(t => t.Cars)
            .HasForeignKey(c => c.TransmissionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.FuelType)
            .WithMany(f => f.Cars)
            .HasForeignKey(c => c.FuelTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasQueryFilter(c => c.IsDeleted == false);
    }
}