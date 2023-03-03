using DataAccess.Entities;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IAuthService
    {
        public Task<ICollection<Claim>> LoginUserAsync(string username, string password);

        public Task RegisterUserAsync(User userdata, string password);

        public Task ConfirmRegistrationAsync(string email, string token);

        public Task<ICollection<Claim>> RefreshCookieAsync(long userId);

        public Task<ICollection<Claim>> ChangePasswordAsync(long activeUserId, string currentPassword, string newPassword);

        public Task RequestEmailResetAsync(long userId, string newEmail);

        public Task ChangeEmailAsync(long userId, string token);

        public Task DeleteAccountAsync(long userId, string currentPassword);

        public Task ResetPasswordRequestAsync(string email);

        public Task ResetPasswordAsync(long userId, string newPassword, string token);
    }
}
