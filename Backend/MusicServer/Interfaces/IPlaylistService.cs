using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;

namespace MusicServer.Interfaces
{
    public interface IPlaylistService
    {
        public Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic, bool receiveNotifications);

        public Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic, bool receiveNotifications);

        public Task SetNotifications(Guid playlistId);

        public Task DeletePlaylistAsync(Guid playlistId);

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<int> orderIds);

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId);

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId);

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos);

        public Task RemoveUsersFromPlaylistAsync(Guid playlistId, List<long> userIds);

        public Task CopyPlaylistToLibraryAsync(Guid playlistId);

        public Task AddPlaylistToLibraryAsync(Guid playlistId);

        public Task<PlaylistPaginationResponse> GetPlaylistsAsync(long userId, int page, int take, string sortAfter, bool asc, string query);

        public Task<ModifieablePlaylistsResponse> GetModifiablePlaylistsAsync(long userId);

        public Task<PlaylistPaginationResponse> GetPublicPlaylistsAsync(int page, int take, string sortAfter, bool asc, string query);

        public Task<PlaylistUserShortDto> GetPlaylistInfoAsync(Guid playlistId);

        public Task<PlaylistSongPaginationResponse> GetSongsInPlaylistAsync(Guid playlistId, int skip, int take, string sortAfter, bool asc, string query);

        public Task<PlaylistSongPaginationResponse> GetFavoritesAsync(int skip, int take, string sortAfter, bool asc, string query);

        public Task AddSongsToFavoriteAsync(List<Guid> songIds);

        public Task RemoveSongsFromFavoriteAsync(List<Guid> songIds);

        public Task ChangeOrderOfFavoritAsync(int oldSpot, int newSpot);

        public Task ChangeOrderOfSongInPlaylistAsync(Guid playlistId, int oldSpot, int newSpot);

        public Task ChangeOrderOfPlaylistAsync(Guid playlistId, int newSpot);

        public Task<int> GetPlaylistSongCountAsync(Guid playlistId);

        public Task<int> GetFavoriteSongCountAsync();
    }
}
