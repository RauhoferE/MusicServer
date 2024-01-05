using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<ArtistDto> GetArtistAsync(Guid artistId);

        public Task<AlbumPaginationResponse> GetAlbumsOfArtistAsync(Guid artistId, int page, int take);

        public Task<AlbumDto> GetAlbumInformationAsync(Guid albumId);

        public Task<SongPaginationResponse> GetSongsInAlbumAsync(Guid albumId, int skip, int take);

        public Task<SongDto> GetSongInformationAsync(Guid songId);

        public Task<SearchResultDto> SearchAsync(string filter, string searchTerm, int page, int take, string sortAfter, bool asc);

        public Task<int> GetSongCountOfAlbumAsync(Guid albumId);
    }
}
