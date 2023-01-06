using DataAccess;
using MusicServer.Entities.DTOs;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class SongService : ISongService
    {
        private readonly MusicServerDBContext dBContext;

        private readonly IActiveUserService activeUserService;

        public SongService(IActiveUserService activeUserService,
            MusicServerDBContext dBContext)
        {
            this.activeUserService = activeUserService;
            this.dBContext = dBContext;
        }

        public Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId)
        {
            throw new NotImplementedException();
        }

        public Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId)
        {
            throw new NotImplementedException();
        }

        public Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public Task DeletePlaylistAsync(Guid playlistId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlaylistAsync(Guid id, string name, string description, bool isPublic)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos)
        {
            throw new NotImplementedException();
        }
    }
}
