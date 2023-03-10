using DataAccess.Entities;
using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface IPlaylistService
    {
        public Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic, bool receiveNotifications);

        public Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic, bool receiveNotifications);

        public Task DeletePlaylistAsync(Guid playlistId);

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId);

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId);

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos);

        public Task RemoveUsersFromPlaylist(Guid playlistId, List<long> userIds);

        public Task CopyPlaylistToLibraryAsync(Guid playlistId);

        public Task AddPlaylistToLibraryAsync(Guid playlistId);

        public Task<PlaylistShortDto[]> GetPlaylistsAsync(int page, int take);

        public Task<PlaylistShortDto[]> GetUserPlaylists(long userId, int page, int take);

        public Task<PlaylistShortDto[]> GetPublicPlaylists(int page, int take);

        public Task<PlaylistDto> GetPlaylistInfo(Guid playlistId);

        public Task<SongDto[]> GetSongsInPlaylist(Guid playlistId, int page, int take);

        public Task<FavoriteDto> GetFavorites(int page, int take);

        public Task AddSongsToFavorite(List<Guid> songIds, bool addClones);

        public Task RemoveSongsFromFavorite(List<Guid> songIds);

    }
}
