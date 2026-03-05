using Serilog;

namespace Ala.Backend.WebAPI.Extensions
{
    public static class SerilogExtensions
    {
        public static void AddSerilogConfiguration(this ConfigureHostBuilder host)
        {
            // Serilog'un kendi hatalarını konsola yazdırarak hata ayıklamayı kolaylaştırır
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

            host.UseSerilog((context, loggerConfiguration) =>
            {
                loggerConfiguration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName() // appsettings'deki {MachineName} için
                    .Enrich.WithThreadId()    // appsettings'deki {ThreadId} için
                    .Enrich.WithProperty("ApplicationName", "Ala.Backend");
            });
        }
    }
}