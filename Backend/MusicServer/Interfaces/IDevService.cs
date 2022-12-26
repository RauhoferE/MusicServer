namespace MusicServer.Interfaces
{
    public interface IDevService
    {
        public Task AddMoqArtistsAlbumsSongsAsync(int numberOfArtists, int numberOfAlbums, int numberOfSongs);
    }
}
