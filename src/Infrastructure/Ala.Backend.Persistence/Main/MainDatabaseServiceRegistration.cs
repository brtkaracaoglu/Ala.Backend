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
            var connectionString = configuration.GetConnectionString("PostgreSQL");

            //Interceptor'ların DI Kaydı
            services.AddScoped<AuditTrackableInterceptor>();
            services.AddScoped<PublishDomainEventsInterceptor>();

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
                    sp.GetRequiredService<AuditTrackableInterceptor>(),
                    sp.GetRequiredService<PublishDomainEventsInterceptor>());

                options.EnableDetailedErrors();

                // Sadece Development ortamında açılması önerilir
                options.EnableSensitiveDataLogging();
            });

            return services;
        }
    }
}