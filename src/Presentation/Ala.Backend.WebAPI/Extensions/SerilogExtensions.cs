using Serilog;

namespace Ala.Backend.WebAPI.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilogConfiguration(this ConfigureHostBuilder host)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
                Console.Error.WriteLine($"Serilog internal error: {msg}")
            );

            host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });
        }
    }
}