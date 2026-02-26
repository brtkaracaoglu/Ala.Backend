using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Persistence.Interceptors;
using Ala.Backend.Persistence.Main;
using Ala.Backend.Persistence.Repositories;
using Ala.Backend.Persistence.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddPostgreSql(configuration);

            // Identity
            services.AddIdentityConfiguration();

            // Sadece UoW kaydı yeterli, repository'leri UoW içinde oluşturuyoruz
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }
    }
}