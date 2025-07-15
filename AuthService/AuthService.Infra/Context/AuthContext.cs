using AuthService.Domain.Entities;
using AuthService.Infra.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infra.Context;

public class AuthContext : DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options) : base(options) {}
    
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}