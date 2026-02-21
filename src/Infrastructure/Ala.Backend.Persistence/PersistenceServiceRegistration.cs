using Ala.Backend.Persistence.Main;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddPostgreSql(configuration);
            services.AddIdentityConfiguration();

            return services;
        }
    }
}