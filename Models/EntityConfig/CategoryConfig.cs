using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DMNRestaurant.Models.EntityConfig
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder
                .Property(c => c.Id)
                .HasColumnType("nvarchar(450)")
                .IsRequired(true);

            builder
                .Property(c => c.Name)
                .HasColumnType("nvarchar(100)")
                .IsRequired(true);

            builder
                .Property(c => c.Description)
                .HasColumnType("nvarchar(500)")
                .IsRequired(false);
        }
    }
}
