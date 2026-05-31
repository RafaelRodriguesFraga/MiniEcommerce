using DotnetBaseKit.Components.Shared.Notifications;
using CustomerService.Infra.Configurations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CustomerService.Infra.Context
{
    public class CustomerContext : BaseContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<Notification>();

            modelBuilder.ApplyConfiguration(new CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {

            configurationBuilder
                .Properties<DateTime>()
                .HaveColumnType("timestamp with time zone")
                .HaveConversion<UtcDateTimeConverter>();
        }
    }

    public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
        {
        }
    }
}