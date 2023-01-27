using AutoMapper;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class SongService : ISongService
    {
        private readonly MusicServerDBContext _dbContext;

        private readonly IActiveUserService _activeUserService;

        private readonly IMapper _mapper;

        public SongService(MusicServerDBContext dbContext, 
            IActiveUserService activeUserService,
            IMapper mapper)
        {
            this._dbContext = dbContext;
            this._activeUserService = activeUserService;
            this._mapper = mapper;
        }

        public async Task<AlbumDto[]> GetAlbumsOfArtist(Guid artistId, int take, int skip)
        {
            var artist = this._dbContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();
            var albums = this._dbContext.Artists
                .Include(x => x.Albums)
                .ThenInclude(x => x.Album)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .Where(x => x.Id == artistId);

            return this._mapper.Map<AlbumDto[]>(albums.ToArray());
        }

        public async Task<ArtistDto> GetArtist(Guid artistId)
        {
            var artist = this._dbContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();
            throw new NotImplementedException();
        }

        public Task<SongDto> GetSongInformation(Guid songId)
        {
            throw new NotImplementedException();
        }

        public Task<SongDto[]> GetSongsInAlbum(Guid albumId, int take, int skip)
        {
            throw new NotImplementedException();
        }
    }
}
