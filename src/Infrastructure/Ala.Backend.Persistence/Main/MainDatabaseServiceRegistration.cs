using Ala.Backend.Persistence.Main.Context;
using Ala.Backend.Persistence.Main.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence.Main
{
    public static class DatabaseServiceRegistration
    {
        public static IServiceCollection AddMainPostgreSql(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MainPostgreSQL")
                ?? throw new InvalidOperationException("Connection string 'MainPostgreSQL' bulunamadı.");

            //Interceptor'ların DI Kaydı
            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<SoftDeleteInterceptor>();

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

                // Not: EF Core ekleme sırasına göre çalıştırır.
                options.AddInterceptors(
                    sp.GetRequiredService<AuditableEntityInterceptor>(),
                    sp.GetRequiredService<SoftDeleteInterceptor>());

                options.EnableDetailedErrors();

                // Sadece Development ortamında açılması önerilir
                options.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}