using CarSellingPlatform.Data.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarSellingPlatform.Data.Configuration;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Seller)
            .WithMany()
            .HasForeignKey(c => c.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder
            .HasOne(c => c.Car)
            .WithMany(c => c.Chats)
            .HasForeignKey(c => c.CarId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasQueryFilter(c => c.Car.IsDeleted == false);
    }
}