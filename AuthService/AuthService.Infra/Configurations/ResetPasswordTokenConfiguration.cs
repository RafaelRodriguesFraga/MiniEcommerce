using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infra.Configurations;

public class ResetPasswordTokenConfiguration : IEntityTypeConfiguration<ResetPasswordToken>
{
    public void Configure(EntityTypeBuilder<ResetPasswordToken> builder)
    {
        builder.Ignore(entity => entity.Notifications);
        builder.Ignore(entity => entity.Valid);
        builder.Ignore(entity => entity.Invalid);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64)
            .HasColumnName("token_hash");

        builder.HasIndex(x => x.TokenHash)
            .IsUnique();

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasColumnName("user_id");

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.ExpirationDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasColumnName("expiration_date");

        builder.Property(x => x.Used)
            .IsRequired()
            .HasDefaultValue(false)
            .HasColumnName("used");

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasColumnName("created_at");

        builder.ToTable("reset_password_tokens");
    }
}