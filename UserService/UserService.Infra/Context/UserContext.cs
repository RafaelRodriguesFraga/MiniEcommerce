using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.EntityFrameworkCore;
using UserService.Infra.Configurations;

namespace UserService.Infra.Context
{
    public class UserContext : BaseContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<Notification>();
        
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
        }
    }
}