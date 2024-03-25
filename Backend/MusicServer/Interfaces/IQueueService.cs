using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.Song;

namespace MusicServer.Interfaces
{
    // TODO: Refactor this to contain less cloned code.
    public interface IQueueService
    {
        // -1, -2, -3 -> Previous played songs
        // 0 -> Current Song
        // 1,2,3 -> Next Songs
        public Task<PlaylistSongDto> CreateQueueAsync(SongDto[] songs, bool orderRandom, int playFromOrder);

        public Task<PlaylistSongDto> CreateQueueAsync(PlaylistSongDto[] songs, bool orderRandom, int playFromOrder);

        // 1. Normal case -> Play every Song In Playlist once and when queue finished return some error (add all the songs from playlist again)
        // 2. Randomize case -> Play every Song In Playlist once and when queue finished return some error( add all the songs from playlist randomized again)

        public Task<PlaylistSongDto> SkipForwardInQueueAsync();

        public Task<PlaylistSongDto> SkipForwardInQueueOfUserAsync(long userId);

        public Task AddSongsToQueueAsync(Guid[] songIds);

        public Task AddSongsToQueueOfUserAsync(long userId, Guid[] songIds);

        public Task<PlaylistSongDto> SkipForwardInQueueAsync(int index);

        public Task<PlaylistSongDto> SkipForwardInQueueOfUserAsync(long userId, int index);

        public Task<PlaylistSongDto> GetCurrentSongInQueueAsync();

        public Task<PlaylistSongDto> GetCurrentSongInQueueOfUserAsync(long userId);

        public Task<PlaylistSongDto> GetSongInQueueWithIndexAsync(int index);

        public Task<PlaylistSongDto> SkipBackInQueueAsync();

        public Task<PlaylistSongDto> SkipBackInQueueOfUserAsync(long userId);

        public Task<QueueSongDto[]> GetCurrentQueueAsync();

        public Task<QueueSongDto[]> GetQueueOfUserAsync(long userId);

        public Task ClearQueueAsync();

        public Task ClearQueueOfUserAsync(long userId);

        public Task ClearManuallyAddedQueueAsync();

        public Task ClearManuallyAddedQueueOfUserAsync(long userId);

        public Task RemoveSongsWithIndexFromQueueAsync(int[] indices);

        public Task RemoveSongsWithIndexFromQueueOfUserAsync(long userId, int[] indices);

        public Task<QueueSongDto[]> PushSongToIndexAsync(int srcIndex, int targetIndex, int markAsAddedManually);

        public Task<QueueSongDto[]> PushSongToIndexOfUserAsync(long userId, int srcIndex, int targetIndex, int markAsAddedManually);

        // This method is called when a song is already playing and the user wants to randomize/derandomize the rest of the queue
        public Task<PlaylistSongDto> ChangeQueueAsync(PlaylistSongDto[] songs, bool randomize);

        // This method is called when a song is already playing and the user wants to randomize the rest of the queue
        public Task<PlaylistSongDto> ChangeQueueAsync(SongDto[] songs, bool randomize);

        public Task UpdateQueueDataAsync(Guid itemId, string loopMode, string sortAfter, string target, bool randomize, bool asc);

        public Task<QueueDataDto> GetQueueDataAsync();
    }
}
