using MusicServer.Core.Interfaces;
using MusicServer.Core.Services;
using MusicServer.Core.Settings;
using MusicServer.Interfaces;
using MusicServer.Services;
using MusicServer.Settings;

namespace MusicServer.Extensions
{
    public static class ServiceInstaller
    {
        public static void InstallControllers(WebApplicationBuilder builder)
        {
            var type = typeof(IServiceInstaller);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            foreach (var t in types)
            {
                var s = (IServiceInstaller)Activator.CreateInstance(t);
                s.InstallService(builder);
            }
        }

        public static void InstallServices(WebApplicationBuilder builder)
        {
            // Add Settings
            builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.Configure<FileserverSettings>(builder.Configuration.GetSection("FileserverSettings"));
            var fileserverCredentials = builder.Configuration.GetSection("FileServerCredentials").Get<FileServerCredentials>();

            builder.Services.AddSingleton(fileserverCredentials);

            // Add Services
            builder.Services.AddTransient<ISftpService, SftpService>();
            builder.Services.AddTransient<IDevService, DevService>();
            builder.Services.AddTransient<IActiveUserService, ActiveUserService>();
            builder.Services.AddTransient<IAuthService, AuthenticationService>();
            builder.Services.AddTransient<IMusicMailService, MusicMailService>();
            builder.Services.AddTransient<IAutomatedMessagingService, AutomatedMessagingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IPlaylistService, PlaylistService>();
            builder.Services.AddTransient<ISongService, SongService>();
            builder.Services.AddTransient<IFileService, FileService>();
            builder.Services.AddTransient<IQueueService, QueueService>();
            builder.Services.AddTransient<IStreamingService, StreamingService>();
            builder.Services.AddTransient<IGroupQueueService, GroupQueueService>();
        }


    }
}
