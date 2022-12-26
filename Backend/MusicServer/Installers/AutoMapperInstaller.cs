using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using MusicServer.Interfaces;
using MusicServer.Mapper;
using System.Reflection;

namespace MusicServer.Installers
{
    public class AutoMapperInstaller : IServiceInstaller
    {
        public void InstallService(WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
