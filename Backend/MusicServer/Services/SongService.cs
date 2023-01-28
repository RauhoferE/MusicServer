using AutoMapper;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using System.Diagnostics;

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

        public async Task<AlbumDto[]> GetAlbumsOfArtist(Guid artistId, int page, int take)
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
            var artist = this._dbContext.Artists
                .Include(x => x.Albums)
                .ThenInclude(x => x.Album)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            return this._mapper.Map<ArtistDto>(artist);
            
        }

        public async Task<SongDto> GetSongInformation(Guid songId)
        {
            var song = this._dbContext.Songs
                .Include(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Album)
                .FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

            return this._mapper.Map<SongDto>(song);
        }

        public async Task<SongDto[]> GetSongsInAlbum(Guid albumId, int page, int take)
        {
            var album = this._dbContext.Albums.FirstOrDefault(x => x.Id == albumId) ?? throw new AlbumNotFoundException();

            var songs = this._dbContext.Songs
                .Include(x => x.Artists)
                .ThenInclude(x=> x.Artist)
                .Include(x => x.Album)
                .Where(x => x.Album.Id == albumId);

            return this._mapper.Map<SongDto[]>(songs.Skip((page - 1) * take).Take(take));
        }

        public Task<SearchResultDto> Search(string filter, int page, int take)
        {
            throw new NotImplementedException();
        }
    }
}
