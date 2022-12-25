using DataAccess;
using DataAccess.Entities;
using Microsoft.Extensions.DependencyInjection;
using MusicServer.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MusicServer.Const;
using Microsoft.AspNetCore.Identity;

namespace MusicServer.Installers
{
    public class AuthenticationInstaller : IServiceInstaller
    {
        public void InstallService(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<MusicServerDBContext>()
            .AddTokenProvider(TokenOptions.DefaultProvider, typeof(EmailTokenProvider<User>));

            // ** Cookies
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddCookie("Cookies", options =>
            {
                options.Cookie.Name = "musicServer_Auth";
                options.Cookie.SameSite = SameSiteMode.Strict;
#if DEBUG
                options.Cookie.SameSite = SameSiteMode.None;
#endif

                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;

                options.LogoutPath = $"/{ApiRoutes.Base}/{ApiRoutes.User.Logout}";
                options.LoginPath = $"/{ApiRoutes.Base}/{ApiRoutes.User.Login}";
                // options.AccessDeniedPath = "/HelloWOrld";

                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        redirectContext.Properties.RedirectUri = $"/{ApiRoutes.Base}/{ApiRoutes.User.Login}";
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },
                    OnRedirectToReturnUrl = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
