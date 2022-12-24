using MusicServer.Interfaces;

namespace MusicServer.Extensions
{
    public static class ServiceInstaller
    {
        public static void InstallServices(WebApplicationBuilder builder)
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


    }
}
