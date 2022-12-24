using MusicServer.Interfaces;
using Serilog;

namespace MusicServer.Installers
{
    public class LoggingInstaller : IServiceInstaller
    {
        public void InstallService(WebApplicationBuilder builder)
        {          
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", false, true)
    .AddEnvironmentVariables();

            var config = configuration.Build();
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(config));
        }
    }
}
