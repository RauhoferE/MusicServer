using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicImporter.Interfaces;
using MusicImporter.Services;
using MusicImporter.Settings;
using MusicServer.Core.Settings;
using MusicServer.Core.Interfaces;
using MusicServer.Core.Services;

var host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(c =>
            {
                var t = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                c.AddEnvironmentVariables();
                c.AddJsonFile("appsettings.json", false, true);
                c.AddJsonFile($"appsettings.{t}.json", false, true);
            })
            .ConfigureServices((context, services) =>
            {
                // Create new Logger and read config from context.
                Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(context.Configuration)
                .CreateLogger();

                services.AddDbContext<MusicServerDBContext>(options =>
                {
                    options.UseSqlServer(context.Configuration.GetConnectionString("DefaultDBConnection"));
                });

                var fileserverSettings = context.Configuration.GetSection("FileserverSettings").Get<FileserverSettings>();
                var musicDataSettings = context.Configuration.GetSection("MusicDataSettings").Get<MusicDataSettings>();
                var fileserverCredentials = context.Configuration.GetSection("FileServerCredentials").Get<FileServerCredentials>();

                services.AddSingleton(fileserverSettings);
                services.AddSingleton(musicDataSettings);
                services.AddSingleton(fileserverCredentials);
                services.AddHttpClient();
                services.AddTransient<IMessageService, MessageService>();
                services.AddTransient<ISftpService, SftpService>();
                services.AddTransient<IFfmpegService, FfmpegService>();
                services.AddTransient<IMusicBrainzService, MusicBrainzService>();
                services.AddTransient<IImportService, ImportService>();
            })
            .UseSerilog()
            .Build();

using (var scope = host.Services.CreateScope())
{
    Log.Information($"Starting import process.");
    var importService = scope.ServiceProvider.GetRequiredService<IImportService>();
    try
    {
        await importService.StartImportProcess();
    }
    catch (DirectoryNotFoundException ex)
    {
        Log.Error($"Couldn't find source directory of songs.");
    }
    catch (Exception ex)
    {
        Log.Error($"Unknown exception occured: {ex.Message}", ex);
    }
    
}