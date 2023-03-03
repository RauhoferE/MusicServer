using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.User;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IUserService
    {
        public Task SubscribeToUser(long userId);

        public Task SuscribeToArtist(Guid artistId);

        public Task UnsubscribeFromUser(long userId);

        public Task UnsubscribeFromArtist(Guid artistId);

        public Task<GuidNameDto[]> GetFollowedArtists(int page, int take);

        public Task<LongNameDto[]> GetFollowedUsers(int page, int take);

        public Task<FullUserDto[]> GetUsersAsync(int page, int take, string searchTerm);   

        public Task<UserDetailsDto> GetUserAsync(long userId);

        public Task ModifyUserAsync(long userId, EditUser userRequest);

        public Task AddRoleToUserAsync(long userId, long roleId);

        public Task RemoveRoleFromUserAsync(long userId, long roleId);

        public Task<LongNameDto[]> GetRoles();
    }
}
