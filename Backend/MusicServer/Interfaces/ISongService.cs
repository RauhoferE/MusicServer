using MusicServer.Entities.DTOs;

namespace MusicServer.Interfaces
{
    public interface ISongService
    {
        public Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic);

        public Task UpdatePlaylistAsync(Guid id, string name, string description, bool isPublic);

        public Task DeletePlaylistAsync(Guid playlistId);

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds);

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId);

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId);

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos);




    }
}
