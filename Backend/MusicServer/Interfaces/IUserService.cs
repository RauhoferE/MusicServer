using DataAccess.Entities;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IUserService
    {
        public Task<ICollection<Claim>> LoginUserAsync(string username, string password);

        public Task RegisterUser(User userdata, string password);

        public Task ConfirmRegistration(string email, string token);

        public Task<ICollection<Claim>> RefreshCookie(Guid userId);
    }
}
