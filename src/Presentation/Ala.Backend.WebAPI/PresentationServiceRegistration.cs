using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.WebAPI.RequestContext;

namespace Ala.Backend.WebAPI
{
    public static class PresentationServiceRegistration
    {
        public static IServiceCollection AddPresentationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, HttpRequestContext>();
                    
            return services;
        }
    }
}
