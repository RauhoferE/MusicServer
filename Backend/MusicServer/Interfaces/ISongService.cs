using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<ArtistDto> GetArtistAsync(Guid artistId);

        public Task<AlbumDto[]> GetAlbumsOfArtistAsync(Guid artistId);

        public Task<AlbumDto> GetAlbumInformationAsync(Guid albumId);

        public Task<SongPaginationResponse> GetSongsInAlbumAsync(Guid albumId, int skip, int take);

        public Task<SongDto> GetSongInformationAsync(Guid songId);

        public Task<SearchResultDto> SearchAsync(string filter, string searchTerm, int page, int take, string sortAfter, bool asc);

        public Task<int> GetSongCountOfAlbumAsync(Guid albumId);
    }
}
