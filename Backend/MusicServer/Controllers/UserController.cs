using AutoMapper;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicServer.Const;
using MusicServer.Entities.Requests.User;
using MusicServer.Interfaces;
using MusicServer.Settings;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly AppSettings appSettings;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route(ApiRoutes.User.Register)]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody, Required] Register register)
        {
            var userdata = this.mapper.Map<Register, User>(register);
            await this.userService.RegisterUser(userdata, register.Password);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.User.Login)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody, Required] Login login)
        {
            var claims = await this.userService.LoginUserAsync(login.Email, login.Password);
            var claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(claimsIdentity);
            await this.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
                    new AuthenticationProperties
                    {
                        IsPersistent =
                        false, // When not persistent == SessionCookie (If browser gets closed the user has to login again)
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.AddHours(this.appSettings.CookieExpirationTimeInMinutes)
            });

            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.Logout)]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.ConfirmMail)]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmMail([FromRoute, Required] string email, [FromRoute, Required] string token)
        {
            await this.userService.ConfirmRegistration(email, token);

            return NoContent();
        }
    }
}
