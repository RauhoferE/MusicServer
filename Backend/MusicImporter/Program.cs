using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicImporter.Interfaces;
using MusicImporter.Services;
using MusicImporter.Settings;

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

                var appSettings = context.Configuration.GetSection("FileserverSettings").Get<FileserverSettings>();

                services.AddSingleton(appSettings);
                services.AddTransient<IImportService, ImportService>();
            })
            .UseSerilog()
            .Build();

using (var scope = host.Services.CreateScope())
{
    //var copyService = scope.ServiceProvider.GetRequiredService<ICopyJobImportService>();
    //await copyService.StartCopyJobImportService();
}