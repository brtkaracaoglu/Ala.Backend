using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence.Main
{
    public static class DatabaseServiceRegistration
    {
        public static IServiceCollection AddPostgreSql(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSQL");

            services.AddDbContext<MainDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Migration assembly ayrı persistence projesinde olmalı
                    npgsqlOptions.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName);

                    // Production için retry policy
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });

                // SQL loglama (sadece development için önerilir)
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging(false);
            });

            return services;
        }
    }
}