using DataAccess;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.Requests.User;
using MusicServer.Extensions;
using MusicServer.Middleware;
using MusicServer.Validation;
using Serilog;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ServiceInstaller.InstallControllers(builder);
        ServiceInstaller.InstallServices(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseCors("MusicServerCorsPolicy");
        
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapControllers();

        app.Run();
    }
}