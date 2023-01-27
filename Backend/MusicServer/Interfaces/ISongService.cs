using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<ArtistDto> GetArtist(Guid artistId);

        public Task<AlbumDto[]> GetAlbumsOfArtist(Guid artistId, int take, int skip);

        public Task<SongDto[]> GetSongsInAlbum(Guid albumId, int take, int skip);

        public Task<SongDto> GetSongInformation(Guid songId);
    }
}
