using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ala.Backend.Persistence.Main
{
    public class MainDesignTimeDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
    {
        public MainDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "../Ala.Backend.WebAPI");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString =
                configuration.GetConnectionString("MainPostgreSQL");

            var optionsBuilder =
                new DbContextOptionsBuilder<MainDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new MainDbContext(optionsBuilder.Options);
        }
    }
}