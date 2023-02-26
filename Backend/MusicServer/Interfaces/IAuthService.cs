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

        public Task<ICollection<Claim>> ChangePasswordAsync(Guid activeUserId, string currentPassword, string newPassword);

        public Task RequestEmailResetAsync(Guid userId, string newEmail);

        public Task ChangeEmailAsync(Guid userId, string token);

        public Task DeleteAccountAsync(Guid userId, string currentPassword);

        public Task ResetPasswordRequestAsync(string email);

        public Task ResetPasswordAsync(Guid userId, string newPassword, string token);
    }
}
