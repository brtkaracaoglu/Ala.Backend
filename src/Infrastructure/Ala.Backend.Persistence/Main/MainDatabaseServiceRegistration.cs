using Ala.Backend.Persistence.Interceptors;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence.Main
{
    public static class DatabaseServiceRegistration
    {
        public static IServiceCollection AddPostgreSql(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("PostgreSQL");

            // Interceptor DI kaydı
            services.AddScoped<AuditDomainEventInterceptor>();

            services.AddDbContext<MainDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(
                        typeof(MainDbContext).Assembly.FullName);

                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null);
                });

                // Interceptor ekleniyor
                options.AddInterceptors(
                    sp.GetRequiredService<AuditDomainEventInterceptor>());

                // Logging
                options.EnableDetailedErrors();

                  //debug
                options.EnableSensitiveDataLogging();

            });

            return services;
        }
    }
}