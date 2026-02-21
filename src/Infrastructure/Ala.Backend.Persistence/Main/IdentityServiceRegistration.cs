using Ala.Backend.Domain.Identity;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ala.Backend.Persistence.Main
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityConfiguration(
            this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>(options =>
                {
                    // USER
                    options.User.RequireUniqueEmail = true;

                    //  PASSWORD POLICY (Enterprise Level)
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredUniqueChars = 3;

                    //  LOCKOUT
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    //  SIGNIN
                    options.SignIn.RequireConfirmedEmail = true; // prod'da true yap
                })
                .AddEntityFrameworkStores<MainDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}