using Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Enitties;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.UnitOfWork;
using Ala.Backend.Application.Abstractions.Persistence.Seeders;
using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Persistence.Main;
using Ala.Backend.Persistence.Main.Repositories.Entities;
using Ala.Backend.Persistence.Main.Repositories.Identity;
using Ala.Backend.Persistence.Main.Repositories.UnitOfWork;
using Ala.Backend.Persistence.Main.Seeders;
using Ala.Backend.Persistence.Main.Services.Identity;
using Ala.Backend.Persistence.Main.Services.Maintenance;
using Ala.Backend.Persistence.Main.Services.Queries;
using Ala.Backend.Persistence.Main.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddMainPostgreSql(configuration);

            // Identity
            services.AddIdentityConfiguration();

            // Sadece UoW kaydı yeterli, repository'leri UoW içinde oluşturuyoruz
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            // Program.cs veya AddPersistenceServices metodu içi
            services.AddScoped(typeof(IReadRepository<,>), typeof(EfReadRepository<,>));
            services.AddScoped<IUserRoleQueryService, UserRoleQueryService>();
            services.AddScoped<IUserQueryService, UserQueryService>();
            services.AddScoped<IRoleQueryService, RoleQueryService>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
            services.AddScoped<IPermissionSeeder, PermissionSeeder>();

            services.Configure<AuthCleanupSettings>(configuration.GetSection(AuthCleanupSettings.SectionName));

            services.AddScoped<IAuthDataCleanupService, AuthDataCleanupService>();

            return services;
        }
    }
}