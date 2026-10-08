using KarinderyaOrderApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KarinderyaOrderApp.Configurations
{
    public class FoodConfiguration : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.Description)
                .HasMaxLength(500);

            builder.Property(f => f.Price)
                .HasPrecision(18, 2);

            builder.Ignore(f => f.IsAvailable);
        }
    }
}
