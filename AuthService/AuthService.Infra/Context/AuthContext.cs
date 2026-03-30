using AuthService.Domain.Entities;
using AuthService.Infra.Configurations;
using DotnetBaseKit.Components.Infra.Sql.Context.Base;
using DotnetBaseKit.Components.Shared.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
        modelBuilder.ApplyConfiguration(new ResetPasswordTokenConfiguration());


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