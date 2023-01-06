using DataAccess.Entities;
using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface IPlaylistService
    {
        public Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic);

        public Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic);

        public Task DeletePlaylistAsync(Guid playlistId);

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId);

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId);

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos);

        public Task CopyPlaylistToLibraryAsync(Guid playlistId);

        public Task<List<PlaylistDto>> GetPlaylistsAsync();

        public Task<List<PlaylistDto>> GetUserPlaylists(Guid userId);

        public Task<List<PlaylistDto>> GetPublicPlaylists();

        public Task<PlaylistDto> GetSongsInPlaylist(Guid playlistId);


    }
}
