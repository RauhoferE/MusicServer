using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface IQueueService
    {
        // TODO: Limit number of Songs to 100
        // -1, -2, -3 -> Previous played songs
        // 0 -> Current Song
        // 1,2,3 -> Next Songs
        //public Task<SongDto[]> CreateQueueFromPlaylist(SongDto[] songs);

        //public Task<SongDto[]> CreateQueueFromFavorites(SongDto[] songs);

        //public Task<SongDto[]> CreateQueueFromAlbum(SongDto[] songs);

        //public Task<SongDto[]> CreateQueueFromArtist(SongDto[] songs);

        public Task<PlaylistSongDto[]> CreateQueue(SongDto[] songs);

        public Task<PlaylistSongDto[]> CreateQueue(PlaylistSongDto[] songs);

        // 1. Normal case -> Play every Song In Playlist once and when queue finished return some error (add all the songs from playlist again)
        // 2. Randomize case -> Play every Song In Playlist once and when queue finished return some error( add all the songs from playlist randomized again)


        
        //public Task<PlaylistSongDto[]> SkipForwardInQueue();

        public Task<PlaylistSongDto> SkipForwardInQueue();

        //public Task<PlaylistSongDto[]> SkipForwardInQueue(int index);

        public Task<PlaylistSongDto> SkipForwardInQueue(int index);

        public Task<PlaylistSongDto> GetCurrentSongInQueue();

        //public Task<PlaylistSongDto[]> SkipBackInQueue();

        public Task<PlaylistSongDto> SkipBackInQueue();

        public Task<PlaylistSongDto[]> RandomizeQueue(SongDto[] songs);

        public Task<PlaylistSongDto[]> RandomizeQueue(PlaylistSongDto[] songs);

        public Task<PlaylistSongDto[]> GetCurrentQueue();

        public Task ClearQueue();

        public Task<PlaylistSongDto[]> RemoveSongsWithIndexFromQueue(List<int> indices);

        public Task<PlaylistSongDto[]> PushSongToIndex(int srcIndex, int targetIndex);
    }
}
