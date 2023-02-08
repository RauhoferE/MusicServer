using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IUserService
    {
        public Task SubscribeToUser(Guid userId);

        public Task SuscribeToArtist(Guid artistId);

        public Task UnsubscribeFromUser(Guid userId);

        public Task UnsubscribeFromArtist(Guid artistId);

        public Task<GuidNameDto[]> GetFollowedArtists(int page, int take);

        public Task<GuidNameDto[]> GetFollowedUsers(int page, int take);
    }
}
