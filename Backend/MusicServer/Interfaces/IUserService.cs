using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.User;
using MusicServer.Entities.Responses;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IUserService
    {
        public Task SubscribeToUserAsync(long userId);

        public Task SuscribeToArtistAsync(Guid artistId);

        public Task UnsubscribeFromUserAsync(long userId);

        public Task UnsubscribeFromArtistAsync(Guid artistId);

        public Task<GuidNamePaginationResponse> GetFollowedArtistsAsync(int page, int take, string query, bool asc);

        public Task<UserDtoPaginationResponse> GetFollowedUsersAsync(int page, int take, string query, bool asc);

        public Task<FullUserPaginationResponse> GetUsersAsyncAsync(int page, int take, string searchTerm, bool asc);

        public Task<AllFollowedEntitiesResponse> GetAllFollowedUsersArtistsPlaylistsFavoritesAsync(string filter, string searchTerm);

        public Task<UserDetailsDto> GetUserAsync(long userId);

        public Task ModifyUserAsync(long userId, EditUser userRequest);

        public Task AddRoleToUserAsync(long userId, long roleId);

        public Task RemoveRoleFromUserAsync(long userId, long roleId);

        public Task<LongNameDto[]> GetRolesAsync();
    }
}
