using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicServer.Extensions;
using MusicServer.Middleware;
using Serilog;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ServiceInstaller.InstallServices(builder);
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ValidationFilter));
            options.Filters.Add(typeof(ExceptionFilter));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}