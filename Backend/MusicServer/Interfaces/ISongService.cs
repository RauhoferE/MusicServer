using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<ArtistDto> GetArtist(Guid artistId);

        public Task<AlbumPaginationResponse> GetAlbumsOfArtist(Guid artistId, int page, int take);

        public Task<AlbumDto> GetAlbumInformation(Guid albumId);

        public Task<SongPaginationResponse> GetSongsInAlbum(Guid albumId, int skip, int take);

        public Task<SongDto> GetSongInformation(Guid songId);

        public Task<SearchResultDto> Search(string filter, string searchTerm, int page, int take, string sortAfter, bool asc);
    }
}
