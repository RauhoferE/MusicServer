using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class AutomatedMessagingService : IAutomatedMessagingService
    {
        public Task AddPlaylistMessage(long userId, Guid playlistId)
        {
            throw new NotImplementedException();
        }

        public Task AddSongsToPlaylistMessage(long userId, Guid playlistId, List<Guid> songIds)
        {
            throw new NotImplementedException();
        }

        public Task PlaylistRemoveMessage(long userId, Guid playlistId, long targetUserId)
        {
            throw new NotImplementedException();
        }

        public Task PlaylistShareMessage(long userId, Guid playlistId, long targetUserId)
        {
            throw new NotImplementedException();
        }

        public Task SendAutomatedMessages()
        {
            throw new NotImplementedException();
        }
    }
}
