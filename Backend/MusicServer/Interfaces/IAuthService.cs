using DataAccess.Entities;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IAuthService
    {
        public Task<ICollection<Claim>> LoginUserAsync(string username, string password);

        public Task RegisterUserAsync(User userdata, string password);

        public Task ConfirmRegistrationAsync(string email, string token);

        public Task<ICollection<Claim>> RefreshCookieAsync(Guid userId);

        // TODO Put in here: Add lock user, delete, add roles, add claims
    }
}
