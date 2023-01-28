using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<ArtistDto> GetArtist(Guid artistId);

        public Task<AlbumDto[]> GetAlbumsOfArtist(Guid artistId, int page, int take);

        public Task<SongDto[]> GetSongsInAlbum(Guid albumId, int page, int take);

        public Task<SongDto> GetSongInformation(Guid songId);

        public Task<SearchResultDto> Search(string filter, int page, int take);
    }
}
