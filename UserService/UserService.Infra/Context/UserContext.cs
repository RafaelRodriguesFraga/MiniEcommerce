
using Microsoft.EntityFrameworkCore;

namespace UserService.Infra.Context
{
    public class BoilerplateContext : DbContext
    {
        public BoilerplateContext(DbContextOptions<BoilerplateContext> options)
            : base(options)
        {
        }

        // Define DbSets for your entities here
        // public DbSet<YourEntity> YourEntities { get; set; }
    }

}