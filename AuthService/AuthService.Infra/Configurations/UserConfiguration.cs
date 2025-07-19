using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infra.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
        builder.Ignore(entity => entity.Notifications);
        builder.Ignore(entity => entity.Valid);
        builder.Ignore(entity => entity.Invalid);
        
        builder.HasKey(x => x.Id).HasName("id");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("email");

        builder.HasIndex(x => x.Email);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasColumnName("password");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.ToTable("users");
    }
}