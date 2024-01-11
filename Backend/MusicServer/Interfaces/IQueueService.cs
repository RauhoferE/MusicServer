using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.Song;

namespace MusicServer.Interfaces
{
    public interface IQueueService
    {
        // TODO: Limit number of Songs to 100
        // -1, -2, -3 -> Previous played songs
        // 0 -> Current Song
        // 1,2,3 -> Next Songs
        public Task<PlaylistSongDto> CreateQueueAsync(SongDto[] songs, bool orderRandom, int playFromOrder);

        public Task<PlaylistSongDto> CreateQueueAsync(PlaylistSongDto[] songs, bool orderRandom, int playFromOrder);

        // 1. Normal case -> Play every Song In Playlist once and when queue finished return some error (add all the songs from playlist again)
        // 2. Randomize case -> Play every Song In Playlist once and when queue finished return some error( add all the songs from playlist randomized again)

        //public Task<PlaylistSongDto[]> SkipForwardInQueue();

        public Task<PlaylistSongDto> SkipForwardInQueueAsync();

        public Task AddSongsToQueueAsync(Guid[] songIds);

        //public Task<PlaylistSongDto[]> SkipForwardInQueue(int index);

        public Task<PlaylistSongDto> SkipForwardInQueueAsync(int index);

        public Task<PlaylistSongDto> GetCurrentSongInQueueAsync();

        public Task<PlaylistSongDto> GetSongInQueueWithIndexAsync(int index);

        //public Task<PlaylistSongDto[]> SkipBackInQueue();

        public Task<PlaylistSongDto> SkipBackInQueueAsync();

        public Task<QueueSongDto[]> GetCurrentQueueAsync();

        public Task ClearQueueAsync();

        public Task ClearManuallyAddedQueueAsync();

        public Task<PlaylistSongDto[]> RemoveSongsWithIndexFromQueueAsync(int[] indices);

        public Task<QueueSongDto[]> PushSongToIndexAsync(int srcIndex, int targetIndex);

        // This method is called when a song is already playing and the user wants to randomize the rest of the queue
        public Task<PlaylistSongDto> RandomizeQueueAsync(PlaylistSongDto[] songs);

        // This method is called when a song is already playing and the user wants to randomize the rest of the queue
        public Task<PlaylistSongDto> RandomizeQueueAsync(SongDto[] songs);

        public Task UpdateQueueDataAsync(Guid itemId, string loopMode, string sortAfter, string target, bool randomize, bool asc);

        public Task<QueueDataDto> GetQueueDataAsync();
    }
}
