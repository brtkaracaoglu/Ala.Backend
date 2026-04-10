using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Ala.Backend.WebAPI.Authentication
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>() ?? new JwtSettings();

            services.Configure<JwtCookieSettings>(
                configuration.GetSection(JwtCookieSettings.SectionName));

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = jwtSettings.RequireHttpsMetadata;
                    options.SaveToken = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Convert.FromBase64String(jwtSettings.SigningKey)),
                        ClockSkew = TimeSpan.FromSeconds(jwtSettings.ClockSkewSeconds),

                        NameClaimType = System.Security.Claims.ClaimTypes.Name,
                        RoleClaimType = System.Security.Claims.ClaimTypes.Role
                    };

                    if (jwtSettings.UseEncryption)
                    {
                        if (string.IsNullOrWhiteSpace(jwtSettings.EncryptionKey))
                            throw new InvalidOperationException(
                                "JwtSettings:EncryptionKey zorunludur çünkü UseEncryption=true.");

                        options.TokenValidationParameters.TokenDecryptionKey =
                            new SymmetricSecurityKey(
                                Convert.FromBase64String(jwtSettings.EncryptionKey));
                    }

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(
                                    JwtCookieNames.AccessToken,
                                    out var accessToken) &&
                                !string.IsNullOrWhiteSpace(accessToken))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        },

                        OnChallenge = async context =>
                        {
                            context.HandleResponse();

                            var requestContext = context.HttpContext.RequestServices.GetService<IRequestContext>();
                            var correlationId = requestContext?.CorrelationId ?? context.HttpContext.TraceIdentifier;

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/problem+json";

                            var problem = new
                            {
                                type = "https://httpstatuses.com/401",
                                title = "Unauthorized",
                                status = 401,
                                detail = "Kimlik doğrulama başarısız.",
                                correlationId
                            };

                            await context.Response.WriteAsJsonAsync(problem);
                        },

                        OnForbidden = async context =>
                        {
                            var requestContext = context.HttpContext.RequestServices.GetService<IRequestContext>();
                            var correlationId = requestContext?.CorrelationId ?? context.HttpContext.TraceIdentifier;

                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/problem+json";

                            var problem = new
                            {
                                type = "https://httpstatuses.com/403",
                                title = "Forbidden",
                                status = 403,
                                detail = "Bu işlem için yetkiniz yok.",
                                correlationId
                            };

                            await context.Response.WriteAsJsonAsync(problem);
                        }
                    };
                });

            return services;
        }
    }
}