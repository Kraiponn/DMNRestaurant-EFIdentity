using DMNRestaurant.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMNRestaurant.Data;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder
            .Entity<User>()
            .Property(u => u.FullName)
            .HasColumnType("nvarchar(120)")
            .IsRequired(false);

        builder
            .Entity<User>()
            .Property(u => u.Photo)
            .HasColumnType("nvarchar(max)")
            .HasDefaultValue("nopic.png")
            .IsRequired(true);

        builder
            .Entity<User>()
            .Property(u => u.Address)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);
    }
}
