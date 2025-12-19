using Microsoft.EntityFrameworkCore;
using ProductService.Domain.Entities;

namespace ProductService.Infra.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Product> builder)
    {
        builder.Ignore(entity => entity.Notifications);
        builder.Ignore(entity => entity.Valid);
        builder.Ignore(entity => entity.Invalid);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("name");

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000) 
            .HasColumnName("description");

        builder.Property(x => x.Category)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("category");

        builder.Property(x => x.ImageUrl)
            .IsRequired()
            .HasMaxLength(300)
            .HasColumnName("image_url");

        builder.Property(x => x.Sku)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("sku");

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("slug");

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 2) 
            .HasColumnName("price");

        builder.Property(x => x.Active)
            .IsRequired()
            .HasColumnName("active");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at");

        builder.HasIndex(x => x.Sku).IsUnique();
        builder.HasIndex(x => x.Slug).IsUnique();

        builder.ToTable("products");
        
    }
}