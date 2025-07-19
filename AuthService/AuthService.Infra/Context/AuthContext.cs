using AuthService.Domain.Entities;
using AuthService.Infra.Configurations;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra.Context;

public class AuthContext : BaseContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<Notification>();
        
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}