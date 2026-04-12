using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Infrastructure.BackgroundJobs;
using Ala.Backend.Infrastructure.Services.Identity;
using Ala.Backend.Infrastructure.Services.Mail;
using Ala.Backend.Infrastructure.Services.Sessions;
using Ala.Backend.Infrastructure.Services.Token;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Ala.Backend.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IValidateOptions<JwtSettings>, JwtSettingsValidator>();
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(JwtSettings.SectionName)).ValidateOnStart();
            services.Configure<MailSettings>(configuration.GetSection(MailSettings.SectionName));

            services.AddScoped<IRefreshTokenHasher, RefreshTokenHasher>();
            services.AddScoped<ITokenLifeCycleService, TokenLifeCycleService>();
            services.AddScoped<ITokenRevocationService, TokenRevocationService>();

            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMailSender, SmtpMailSender>();
            services.AddScoped<ITemplateService, TemplateService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserSessionService, UserSessionService>();
            services.AddScoped<IClientInfoParser, UaClientInfoParser>();

            services.Configure<AuthCleanupSettings>(
          configuration.GetSection(AuthCleanupSettings.SectionName));

            services.AddHostedService<AuthCleanupBackgroundService>();
            return services;
        }
    }
}