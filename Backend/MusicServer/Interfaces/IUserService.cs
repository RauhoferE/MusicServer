using DataAccess.Entities;
using System.Security.Claims;

namespace MusicServer.Interfaces
{
    public interface IUserService
    {
        public Task SubscribeToUser(Guid userId);

        public Task SuscribeToArtist(Guid artistId);

        public Task UnsubscribeFromUser(Guid userId);

        public Task UnsubscribeFromArtist(Guid artistId);
    }
}
