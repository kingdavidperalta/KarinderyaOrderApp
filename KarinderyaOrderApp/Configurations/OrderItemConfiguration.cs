using KarinderyaOrderApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarinderyaOrderApp.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            builder.Ignore(oi => oi.LineTotal);

            builder.HasOne(oi => oi.Food)
                .WithMany(f => f.OrderItems)
                .HasForeignKey(oi => oi.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
