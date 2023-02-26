using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Entities.Requests.User;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MusicServer.Interfaces;
using MusicServer.Settings;
using AutoMapper;
using MusicServer.Services;
using Microsoft.Extensions.Options;
using DataAccess.Entities;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class AuthenticationController : Controller
    {
        private readonly IMapper mapper;
        private readonly AppSettings appSettings;
        private readonly IAuthService authService;

        public AuthenticationController(
            IAuthService authService,
            IMapper mapper,
            IOptions<AppSettings> appSettings
            )
        {
            this.mapper = mapper;
            this.authService = authService;
            this.appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.Register)]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody, Required] Register register)
        {
            var userdata = this.mapper.Map<Register, User>(register);
            await this.authService.RegisterUserAsync(userdata, register.Password);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.Login)]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody, Required] Login login)
        {
            var claims = await this.authService.LoginUserAsync(login.Email, login.Password);
            await this.AddClaimsCookie(claims);

            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Authentication.Logout)]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await this.RemoveClaimsCookie();

            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Authentication.ConfirmMail)]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmMail([FromRoute, Required] string email, [FromRoute, Required] string token)
        {
            await this.authService.ConfirmRegistrationAsync(email, token);

            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.ChangePassword)]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody, Required] ChangePassword request)
        {
            var userIdClaim = this.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier); 

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            var claims = await this.authService.ChangePasswordAsync(Guid.Parse(userIdClaim.Value), request.CurrentPassword, request.NewPassword);
            await this.RemoveClaimsCookie();
            await this.AddClaimsCookie(claims);

            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.ForgetPassword)]
        [AllowAnonymous]
        public async Task<IActionResult> ForgetPassword([FromBody, Required] ChangeEmail request)
        {
            await this.authService.ResetPasswordRequestAsync(request.Email);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.ResetPassword)]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromRoute, Required] Guid userId, [FromRoute, Required] string token, [FromBody, Required] ResetPassword request)
        {
            await this.authService.ResetPasswordAsync(userId, request.Password, token);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Authentication.RequestEmailReset)]
        [Authorize]
        public async Task<IActionResult> RequestEmailReset([FromBody, Required] ChangeEmail request)
        {
            var userIdClaim = this.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            await this.authService.RequestEmailResetAsync(Guid.Parse(userIdClaim.Value), request.Email);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Authentication.ChangeEmail)]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeEmail([FromRoute, Required] Guid userId, [FromRoute, Required] string token)
        {
            await this.authService.ChangeEmailAsync(userId, token);
            await this.RemoveClaimsCookie();
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Authentication.DeleteAccount)]
        [Authorize]
        public async Task<IActionResult> DeleteAccount([FromBody, Required] DeleteAccount request)
        {
            var userIdClaim = this.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return BadRequest();
            }

            await this.authService.DeleteAccountAsync(Guid.Parse(userIdClaim.Value), request.Password);
            await this.RemoveClaimsCookie();
            return NoContent();
        }

        private async Task RemoveClaimsCookie()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task AddClaimsCookie(IEnumerable<Claim> claims)
        {
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
        }
    }
}
