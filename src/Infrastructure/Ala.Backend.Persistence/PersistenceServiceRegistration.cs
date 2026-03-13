using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Persistence.Main;
using Ala.Backend.Persistence.Main.Repositories.Identity;
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

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRolesRepository, UserRolesRepository>();
            return services;
        }
    }
}