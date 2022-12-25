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
            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddMaps(Assembly.GetExecutingAssembly());
            //    //cfg.AddProfile<DtoToResponse>();
            //    //cfg.AddProfile<RequestToDto>();
            //});
            //var c = config.CreateMapper();

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
