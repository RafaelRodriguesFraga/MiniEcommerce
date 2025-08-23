using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.EntityFrameworkCore;
using CustomerService.Infra.Configurations;

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
        }
    }
}