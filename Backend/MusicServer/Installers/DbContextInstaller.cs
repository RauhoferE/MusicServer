using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicServer.Interfaces;

namespace MusicServer.Installers
{
    public class DbContextInstaller : IServiceInstaller
    {
        public void InstallService(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<MusicServerDBContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDBConnection")));
        }
    }
}
