using DataAccess;
using FluentValidation;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.Requests.User;
using MusicServer.Extensions;
using MusicServer.HubFilters;
using MusicServer.Hubs;
using MusicServer.Interfaces;
using MusicServer.Middleware;
using MusicServer.Validation;
using Serilog;
using System.Runtime.CompilerServices;
using System.Timers;

internal class Program
{
    private static WebApplication APP;

    private static readonly TaskFactory TASK_FACTORY = new TaskFactory();

    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ServiceInstaller.InstallControllers(builder);
        ServiceInstaller.InstallServices(builder);
        builder.Services.AddSignalR(options =>
        {
            options.AddFilter<ErrorFilter>();
        });

        var app = builder.Build();

        APP = app;
#if !DEBUG
        var messagingTimer = new System.Timers.Timer(100);
        messagingTimer.Elapsed += SendAutomatedEmails;
        messagingTimer.AutoReset = false;
        messagingTimer.Start();
#endif

        // Configure the HTTP request pipeline.
        app.UseCors("MusicServerCorsPolicy");
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.MapControllers();
        app.MapHub<StreamingHub>("streaming");

        app.Run();
    }

    private static async void SendAutomatedEmails(object source, ElapsedEventArgs e)
    {
        await TASK_FACTORY.StartNew(async () =>
        {
            using var scope = APP.Services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IAutomatedMessagingService>().SendAutomatedMessagesAsync();
        });
    }


}