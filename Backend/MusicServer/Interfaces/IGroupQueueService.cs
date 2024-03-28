using DataAccess.Entities;
using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface IGroupQueueService
    {
        // -1, -2, -3 -> Previous played songs
        // 0 -> Current Song
        // 1,2,3 -> Next Songs
        public Task<PlaylistSongDto> CreateQueueAsync(Guid groupName, SongDto[] songs, bool orderRandom, int playFromOrder);

        public Task<PlaylistSongDto> CreateQueueAsync(Guid groupName, PlaylistSongDto[] songs, bool orderRandom, int playFromOrder);

        // 1. Normal case -> Play every Song In Playlist once and when queue finished return some error (add all the songs from playlist again)
        // 2. Randomize case -> Play every Song In Playlist once and when queue finished return some error( add all the songs from playlist randomized again)

        public Task<PlaylistSongDto> SkipForwardInQueueAsync(Guid groupName);

        public Task AddSongsToQueueAsync(Guid groupName, Guid[] songIds);

        public Task<PlaylistSongDto> SkipForwardInQueueAsync(Guid groupName, int index);


        public Task<PlaylistSongDto> GetCurrentSongInQueueAsync(Guid groupName);

        public Task<PlaylistSongDto> GetSongInQueueWithIndexAsync(Guid groupName, int index);

        public Task<PlaylistSongDto> SkipBackInQueueAsync(Guid groupName);

        public Task<QueueSongDto[]> GetCurrentQueueAsync(Guid groupName);

        public Task<QueueSongDto[]> MarkQueueSongsAsFavorite(long userID, QueueSongDto[] songs);

        public Task<PlaylistSongDto> MarkPlaylistSongAsFavorite(long userID, PlaylistSongDto song);

        public Task ClearQueueAsync(Guid groupName);

        public Task ClearManuallyAddedQueueAsync(Guid groupName);

        public Task RemoveSongsWithIndexFromQueueAsync(Guid groupName, int[] indices);

        public Task<QueueSongDto[]> PushSongToIndexAsync(Guid groupName, int srcIndex, int targetIndex, int markAsAddedManually);

        // This method is called when a song is already playing and the user wants to randomize/derandomize the rest of the queue
        public Task<PlaylistSongDto> ChangeQueueAsync(Guid groupName, PlaylistSongDto[] songs, bool randomize);

        // This method is called when a song is already playing and the user wants to randomize the rest of the queue
        public Task<PlaylistSongDto> ChangeQueueAsync(Guid groupName, SongDto[] songs, bool randomize);

        public Task UpdateQueueDataAsync(Guid groupName, Guid itemId, string loopMode, string sortAfter, string target, bool randomize, bool asc);

        public Task<QueueDataDto> GetQueueDataAsync(Guid groupName);

        public Task SetQueueDataAsync(QueueData queueData);

        public Task SetQueueEntitiesAsync(QueueEntity[] queueEntities);

        public Task RemoveQueueDataAndEntitiesAsync(Guid groupName);
    }
}
