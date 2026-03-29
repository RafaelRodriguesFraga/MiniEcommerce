using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;

namespace ProductService.Infra.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
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

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnName("slug");

            builder.Property(x => x.Active)
                .IsRequired()
                .HasColumnName("active");

            builder.Property(x => x.CreatedAt)
                .IsRequired()
                .HasColumnName("created_at");

            builder.Property(x => x.UpdatedAt)
                .IsRequired()
                .HasColumnName("updated_at");

            builder.HasIndex(x => x.Slug).IsUnique();

            builder.ToTable("categories");
        }
    }
}