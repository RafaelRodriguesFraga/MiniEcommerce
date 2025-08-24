namespace CustomerService.Infra.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.Ignore(entity => entity.Notifications);
        builder.Ignore(entity => entity.Valid);
        builder.Ignore(entity => entity.Invalid);

        builder.HasKey(x => x.Id).HasName("id");

        builder.Property(x => x.Id)
            .IsRequired()
            .HasColumnName("id");

        builder.Property(x => x.CustomerId)
            .IsRequired()
            .HasColumnName("customer_id");

        builder.Property(x => x.Street)
            .IsRequired()
            .HasMaxLength(150)
            .HasColumnName("street");

        builder.Property(x => x.Number)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("number");

        builder.Property(x => x.Neighborhood)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("neighborhood");

        builder.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("city");

        builder.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("state");

        builder.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("postal_code");

        builder.Property(x => x.Complement)
            .HasMaxLength(100)
            .HasColumnName("complement")
            .IsRequired(false);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(x => x.UpdatedAt)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
        
        builder.HasOne(address => address.Customer)
            .WithMany(customer => customer.Addresses)
            .HasForeignKey(address => address.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("addresses");
    }
}