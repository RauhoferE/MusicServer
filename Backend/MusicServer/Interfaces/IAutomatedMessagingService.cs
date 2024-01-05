using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IAutomatedMessagingService
    {
        Task SendAutomatedMessagesAsync();

        Task AddPlaylistMessageAsync(long userId, Guid playlistId);

        Task AddSongsToPlaylistMessageAsync(long userId, Guid playlistId, List<Guid> songIds);

        Task PlaylistShareMessageAsync(long userId, Guid playlistId, long targetUserId);

        Task PlaylistRemoveMessageAsync(long userId, Guid playlistId, long targetUserId);


    }
}
