using Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ala.Backend.Infrastructure.BackgroundJobs
{
    public sealed class AuthCleanupBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AuthCleanupBackgroundService> _logger;
        private readonly AuthCleanupSettings _settings;

        public AuthCleanupBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<AuthCleanupBackgroundService> logger,
            IOptions<AuthCleanupSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = TimeSpan.FromHours(_settings.IntervalHours);
            var initialDelay = TimeSpan.FromMinutes(5);

            await Task.Delay(initialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var cleanupService = scope.ServiceProvider
                        .GetRequiredService<IAuthDataCleanupService>();

                    var result = await cleanupService.CleanupAsync(stoppingToken);

                    _logger.LogInformation(
                        "Auth cleanup completed at {ExecutedAtUtc}. Deleted refresh tokens: {RefreshTokens}, deleted sessions: {Sessions}",
                        result.ExecutedAtUtc,
                        result.DeletedRefreshTokens,
                        result.DeletedUserSessions);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Auth cleanup job failed.");
                }

                await Task.Delay(interval, stoppingToken);
            }

        }
    }
}