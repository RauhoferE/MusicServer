using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IAutomatedMessagingService
    {
        Task SendAutomatedMessages();

        Task AddPlaylistMessage(long userId, Guid playlistId);

        Task AddSongsToPlaylistMessage(long userId, Guid playlistId, List<Guid> songIds);

        Task PlaylistShareMessage(long userId, Guid playlistId, long targetUserId);

        Task PlaylistRemoveMessage(long userId, Guid playlistId, long targetUserId);


    }
}
