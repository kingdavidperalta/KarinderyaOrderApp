using KarinderyaOrderApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarinderyaOrderApp.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Ignore(o => o.TotalQuantity);
            builder.Ignore(o => o.TotalPrice);
        }
    }
}
