using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using MusicServer.Entities.Requests.User;
using MusicServer.Interfaces;
using MusicServer.Middleware;
using MusicServer.Validation;
using System;
using System.Reflection;

namespace MusicServer.Installers
{
    public class MVCInstaller : IServiceInstaller
    {
        public void InstallService(WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddCors(x =>
            {
                x.AddPolicy("MusicServerCorsPolicy", policy =>
                {
                    //TODO: Put exposed headers here
                    policy.WithOrigins(builder.Configuration.GetSection("AppSettings:AllowedCorsHosts").Get<string[]>())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            builder.Services.AddMvc(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
            var t = Assembly.GetExecutingAssembly();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ExceptionFilter>();

            });

            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddFluentValidationAutoValidation();

        }
    }
}
