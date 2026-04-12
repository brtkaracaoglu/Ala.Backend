using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Presentation.Abstractions;
using Ala.Backend.WebAPI.Authentication;
using Ala.Backend.WebAPI.Filters;
using Ala.Backend.WebAPI.RequestContext;
using Ala.Backend.WebAPI.Services;

namespace Ala.Backend.WebAPI
{
    public static class PresentationServiceRegistration
    {
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, HttpRequestContext>();
            services.AddScoped<ITokenCookieService, TokenCookieService>();
            services.AddScoped<TokenCookieFilter>();

            services.AddControllers();


            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();

            return services;
        }
    }
}