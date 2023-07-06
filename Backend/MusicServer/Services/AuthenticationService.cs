using DataAccess.Entities;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Serilog;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Const;

namespace MusicServer.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly MusicServerDBContext dBContext;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMusicMailService mailService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AuthenticationService(UserManager<User> userManager,
    MusicServerDBContext dbContext,
    SignInManager<User> signInManager,
    RoleManager<Role> roleManager,
    IMusicMailService mailService,
    IHttpContextAccessor httpContextAccessor)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
            this.dBContext = dbContext;
            this.mailService = mailService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task ChangeEmailAsync(long userId, string token)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId)
                    ?? throw new UserNotFoundException("User not found.");

            if (user.TemporarayEmail == null)
            {
                throw new MusicserverServiceException("Error when changing email.");
            }

            if (!(await this._userManager.ChangeEmailAsync(user, user.TemporarayEmail, token)).Succeeded)
            {
                throw new MusicserverServiceException("Error when changing email.");
            }
        }

        public async Task ConfirmRegistrationAsync(string email, string token)
        {
            var user = await this._userManager.FindByEmailAsync(email) ?? throw new UserNotFoundException();

            var res = await this._userManager.ConfirmEmailAsync(user, token);

            if (!res.Succeeded)
            {
                throw new MusicserverServiceException("Couldn't confirm email: " + string.Join(", ", res.Errors.Select(x => x.Description)));
            }

            Log.Information($"User Email confirmed: {email}");
        }

        public async Task DeleteAccountAsync(long userId, string currentPassword)
        {
            var user = this.dBContext.Users.Include(x => x.FollowedArtists)
                .Include(x => x.FollowedUsers)
                .Include(x => x.UserRoles)
                .FirstOrDefault(x => x.Id == userId)
                ?? throw new UserNotFoundException("User not found.");

            var playlists = this.dBContext.PlaylistUsers
                .Include(x => x.User)
                .Include(x => x.Playlist)
                .Where(x => x.User.Id == userId && x.IsCreator)
                .ToList()
                .DistinctBy(x => x.Playlist.Id)
                .Select(x => x.Playlist);

            this.dBContext.RemoveRange(playlists);

            await this._userManager.DeleteAsync(user);

            // Send Email that Account has been deleted
        }

        public async Task<LoginUserClaimsResult> LoginUserAsync(string username, string password)
        {
            // Get user
            var user = await this._userManager.FindByEmailAsync(username) ?? throw new UserNotFoundException();

            // Check Mail confirmed
            if (!user.EmailConfirmed)
            {
                throw new MusicserverServiceException("Please confirm your registration.");
            }

            // Check login
            var loginCheck = await this._signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!loginCheck.Succeeded)
            {
                throw new MusicserverServiceException("Login failed. Check username and password.");
            }

            // Create claimsList
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var frontendClaims = new List<Claim>
            {
                new("name", user.Email),
            };

            // Create rolesList
            var roles = await this._userManager.GetRolesAsync(user);
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
            roles.ToList().ForEach(role => frontendClaims.Add(new Claim("roles", role)));
            var u = this.dBContext.Users.FirstOrDefault(x => x.Id == user.Id) ?? throw new UserNotFoundException();
            u.LastLogin = DateTime.Now;
            await this.dBContext.SaveChangesAsync();

            return new LoginUserClaimsResult
            {
                AuthenticationClaims = claims,
                FrontendClaims = frontendClaims
            };
        }

        public async Task<ICollection<Claim>> RefreshCookieAsync(long userId)
        {
            // Get user
            var user = await this._userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException();

            // Create claimsList
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            // Create rolesList
            var roles = await this._userManager.GetRolesAsync(user);
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));


            return claims;
        }

        public async Task RegisterUserAsync(User userdata, string password, Guid registrationCode)
        {
            var user = await this._userManager.FindByEmailAsync(userdata.Email);
            if (user != null)
            {
                throw new MusicserverServiceException("User already exists");
            }

            var registrationCodeEntity = this.dBContext.RegistrationCodes
                .FirstOrDefault(x => x.Id == registrationCode && 
                x.UsedDate == null &&
                x.UsedByEmail == null) ?? throw new MusicserverServiceException("RegistrationCode is invalid");

            registrationCodeEntity.UsedDate = DateTime.Now;
            registrationCodeEntity.UsedByEmail = userdata.Email;

            var result = await this._userManager.CreateAsync(userdata, password);

            if (!result.Succeeded)
            {
                throw new MusicserverServiceException("User couldn't be created: " + string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            user = await this._userManager.FindByEmailAsync(userdata.Email) ?? throw new UserNotFoundException();

            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

            Log.Information($"Created email token for new user: {userdata.Email}");

            await this.dBContext.SaveChangesAsync();

            //TOOD: Change address to frontend
            await this.mailService.SendWelcomeEmail(user, 
                $"https://{this.httpContextAccessor.HttpContext.Request.Host.Value}{ApiRoutes.Authentication.ConfirmMail.Replace("{email}", user.Email).Replace("{token}", token)}");
        }

        public async Task RequestEmailResetAsync(long userId, string newEmail)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId)
                    ?? throw new UserNotFoundException("User not found.");

            var token = await this._userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            user.TemporarayEmail = newEmail;
            await this.dBContext.SaveChangesAsync();

            //TOOD: Change address to frontend
            await this.mailService.SendEmailChangeEmail(user,
    $"https://{this.httpContextAccessor.HttpContext.Request.Host.Value}{ApiRoutes.Authentication.ChangeEmail.Replace("{userId}", user.Id.ToString()).Replace("{token}", token)}");
        }

        public async Task ResetPasswordRequestAsync(string email)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Email == email)
    ?? throw new UserNotFoundException("User not found.");

            var token = await this._userManager.GeneratePasswordResetTokenAsync(user);

            //TOOD: Change address to frontend
            await this.mailService.SendPasswordResetEmail(user,
$"https://{this.httpContextAccessor.HttpContext.Request.Host.Value}{ApiRoutes.Authentication.ResetPassword.Replace("{userId}", user.Id.ToString()).Replace("{token}", token)}");
        }

        public async Task ResetPasswordAsync(long userId, string newPassword, string token)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId)
    ?? throw new UserNotFoundException("User not found.");

            if (!(await this._userManager.ResetPasswordAsync(user, token, newPassword)).Succeeded)
            {
                throw new MusicserverServiceException("Error when reseting the password.");
            }
        }

        public async Task<ICollection<Claim>> ChangePasswordAsync(long activeUserId, string currentPassword, string newPassword)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == activeUserId) 
                ?? throw new UserNotFoundException("User not found.");

            if (!(await this._userManager.ChangePasswordAsync(user, currentPassword, newPassword)).Succeeded)
            {
                throw new MusicserverServiceException("Error when changing the password.");
            }

            return (await this.LoginUserAsync(user.Email, newPassword)).AuthenticationClaims;
        }



        public async Task<Guid[]> GenerateRegistrationCodesAsync(int amount)
        {
            List<Guid> registrationCodes = new List<Guid>();
            for (int i = 0; i < amount; i++)
            {
                registrationCodes.Add(this.dBContext.RegistrationCodes.Add(new RegistrationCode()).Entity.Id);
            }

            await this.dBContext.SaveChangesAsync();
            return registrationCodes.ToArray();
        }


    }
}
