using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ala.Backend.Persistence.Main.Context
{
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();

            // Changed to UseNpgsql to match your app's actual database provider.
            // Replace with your local PostgreSQL development connection string.
            optionsBuilder.UseNpgsql("Host=localhost;Port=5430;Database=AlaBackendWebAPI;Username=postgres;Password=sCd06;");

            return new MainDbContext(optionsBuilder.Options);
        }
    }
}