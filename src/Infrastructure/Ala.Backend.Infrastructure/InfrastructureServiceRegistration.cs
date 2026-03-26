using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Infrastructure.Services.Identity;
using Ala.Backend.Infrastructure.Services.Mail;
using Ala.Backend.Infrastructure.Services.Token;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<ITokenService, TokenService>();

            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IMailSender, SmtpMailSender>();
            services.AddScoped<ITemplateService, TemplateService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            return services;
        }
    }
}