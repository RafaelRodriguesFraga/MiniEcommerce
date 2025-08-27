namespace CustomerService.Infra.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Ignore(entity => entity.Notifications);
        builder.Ignore(entity => entity.Valid);
        builder.Ignore(entity => entity.Invalid);

        builder.HasKey(x => x.Id).HasName("PK_Customers");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(x => x.AuthServiceId)
            .IsRequired()
            .HasColumnName("auth_service_id");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("name");

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("email");

        builder.Property(x => x.AvatarUrl)
            .HasColumnName("avatar_url")
            .IsRequired(false);

        builder.HasIndex(x => x.Email);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.ToTable("customers");
    }
}