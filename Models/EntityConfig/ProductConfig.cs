using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMNRestaurant.Models.EntityConfig
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder
                .Property(p => p.Id)
                .HasColumnType("nvarchar(455)");

            builder
                .Property(p => p.Name)
                .HasColumnType("nvarchar(100)")
                .IsRequired();

            builder
                .Property(p => p.Description)
                .HasColumnType("nvarchar(500)")
                .IsRequired();

            builder
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal(10,2)")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.InStock)
                .HasColumnType("int")
                .HasDefaultValue(0)
                .IsRequired();

            builder
                .Property(p => p.Photo)
                .HasColumnType("nvarchar(450)")
                .HasDefaultValue("nopic.png")
                .IsRequired();

            builder
                .HasOne(c => c.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
