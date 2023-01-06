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

            // Add Services
            builder.Services.AddTransient<IDevService, DevService>();
            builder.Services.AddTransient<IActiveUserService, ActiveUserService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<ISongService, SongService>();
        }


    }
}
