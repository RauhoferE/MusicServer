using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Serilog;
using System.Security.Claims;

namespace MusicServer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly MusicServerDBContext dBContext;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public UserService(UserManager<User> userManager,
            MusicServerDBContext dbContext,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager)
        {
            this._userManager= userManager;
            this._roleManager= roleManager;
            this._signInManager= signInManager;
            this.dBContext= dbContext;
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

        public async Task<ICollection<Claim>> LoginUserAsync(string username, string password)
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

            // Create rolesList
            var roles = await this._userManager.GetRolesAsync(user);
            roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
            var u = this.dBContext.Users.FirstOrDefault(x => x.Id == user.Id) ?? throw new UserNotFoundException();
            u.LastLogin = DateTime.Now;

            return claims;
        }

        public async Task<ICollection<Claim>> RefreshCookieAsync(Guid userId)
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

        public async Task RegisterUserAsync(User userdata, string password)
        {
            var user = await this._userManager.FindByEmailAsync(userdata.Email);
            if (user != null)
            {
                throw new MusicserverServiceException("User already exists");
            }

            var result = await this._userManager.CreateAsync(userdata, password);

            if (!result.Succeeded)
            {
                throw new MusicserverServiceException("User couldn't be created: " + string.Join(", ", result.Errors.Select(x => x.Description)));
            }

            user = await this._userManager.FindByEmailAsync(userdata.Email) ?? throw new UserNotFoundException();

            var token = await this._userManager.GenerateEmailConfirmationTokenAsync(user);

            Log.Information($"Created email token for new user: {userdata.Email}");

            // Send token per email
        }
    }
}
