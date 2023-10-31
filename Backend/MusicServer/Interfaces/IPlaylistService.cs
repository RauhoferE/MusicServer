using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;

namespace MusicServer.Interfaces
{
    public interface IPlaylistService
    {
        public Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic, bool receiveNotifications);

        public Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic, bool receiveNotifications);

        public Task DeletePlaylistAsync(Guid playlistId);

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<int> orderIds);

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId);

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId);

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos);

        public Task RemoveUsersFromPlaylist(Guid playlistId, List<long> userIds);

        public Task CopyPlaylistToLibraryAsync(Guid playlistId);

        public Task AddPlaylistToLibraryAsync(Guid playlistId);

        public Task<PlaylistPaginationResponse> GetPlaylistsAsync(long userId, int page, int take, string sortAfter, bool asc, string query);

        public Task<PlaylistPaginationResponse> GetPublicPlaylists(int page, int take, string sortAfter, bool asc, string query);

        public Task<PlaylistUserShortDto> GetPlaylistInfo(Guid playlistId);

        public Task<PlaylistSongPaginationResponse> GetSongsInPlaylist(Guid playlistId, int page, int take, string sortAfter, bool asc, string query);

        public Task<PlaylistSongPaginationResponse> GetFavorites(int page, int take, string sortAfter, bool asc, string query);

        public Task AddSongsToFavorite(List<Guid> songIds);

        public Task RemoveSongsFromFavorite(List<Guid> songIds);

        public Task ChangeOrderOfFavorit(Guid songId, int newSpot);

        public Task ChangeOrderOfSongInPlaylist(Guid playlistId, Guid songId, int newSpot);

        public Task ChangeOrderOfPlaylist(Guid playlistId, int newSpot);
    }
}
