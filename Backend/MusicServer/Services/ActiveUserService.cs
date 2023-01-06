using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using MusicServer.Interfaces;
using System.Security.Claims;

namespace MusicServer.Services
{
    public class ActiveUserService : IActiveUserService
    {
        private readonly User user;

        public ActiveUserService(HttpContextAccessor contextAccessor,
            UserManager<User> userManager)
        {
            if (contextAccessor.HttpContext?.User.Identity == null)
            {
                return;
            }

            this.user = userManager.FindByEmailAsync(contextAccessor.HttpContext?.User.Identity.Name).Result;

            if (this.user == null)
            {
                return;
            }

            this.Roles = userManager.GetRolesAsync(this.user).Result.ToList();
        }

        public string Email => this.user.Email;

        public Guid Id => this.user.Id;

        public bool IsNull => this.user == null;

        public List<string> Roles { get; }
    }
}
