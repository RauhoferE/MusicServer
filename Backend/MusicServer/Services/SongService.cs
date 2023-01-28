using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.User;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using System.Diagnostics;
using System.Linq;
using static MusicServer.Const.ApiRoutes;

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

        public async Task<SearchResultDto> Search(string filter, string searchTerm, int page, int take)
        {
            switch (filter)
            {
                case SearchFilter.All:
                    return await this.FilterAll(searchTerm, page, take);
                case SearchFilter.Albums:
                    return await this.FilterAllAlbums(searchTerm, page, take);
                case SearchFilter.Artists:
                    return await this.FilterAllArtists(searchTerm, page, take);
                case SearchFilter.Songs:
                    return await this.FilterAllSongs(searchTerm, page, take);
                case SearchFilter.Playlists:
                    return await this.FilterAllPlaylists(searchTerm, page, take);
                case SearchFilter.Users:
                    return await this.FilterAllUsers(searchTerm, page, take);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<SearchResultDto> FilterAll(string searchTerm, int page, int take)
        {
            var songs = this._dbContext.Songs.Where(x => true);

            var albums = this._dbContext.Albums.Where(x => true);

            var artists = this._dbContext.Artists.Where(x => true);

            var playlists = this._dbContext.Playlists.Where(x => true);

            var users = this._dbContext.Users.Where(x => true);

            if (searchTerm != string.Empty)
            {
                songs = songs.Where(x => x.Name.Contains(searchTerm));
                albums = albums.Where(x => x.Name.Contains(searchTerm));
                artists = artists.Where(x => x.Name.Contains(searchTerm));
                playlists = playlists.Where(x => x.Name.Contains(searchTerm));
                users = users.Where(x => x.UserName.Contains(searchTerm));
            }

            return new SearchResultDto()
            {
                Songs = this._mapper.Map<SongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray()),
                Albums = this._mapper.Map<AlbumDto[]>(albums.Skip((page - 1) * take).Take(take).ToArray()),
                Artists = this._mapper.Map<GuidNameDto[]>(artists.Skip((page - 1) * take).Take(take).ToArray()),
                Playlists = this._mapper.Map<PlaylistShortDto[]>(playlists.Skip((page - 1) * take).Take(take).ToArray()),
                Users = this._mapper.Map<UserDto[]>(users.Skip((page - 1) * take).Take(take).ToArray())
            };
        }

        private async Task<SearchResultDto> FilterAllAlbums(string searchTerm, int page, int take)
        {
            var albums = this._dbContext.Albums.Where(x => true);

            if (searchTerm != string.Empty)
            {
                albums = albums.Where(x => x.Name.Contains(searchTerm));
            }

            return new SearchResultDto()
            {
                Albums = this._mapper.Map<AlbumDto[]>(albums.Skip((page - 1) * take).Take(take).ToArray()),
            };
        }

        private async Task<SearchResultDto> FilterAllArtists(string searchTerm, int page, int take)
        {
            var artists = this._dbContext.Artists.Where(x => true);

            if (searchTerm != string.Empty)
            {
                artists = artists.Where(x => x.Name.Contains(searchTerm));
            }

            return new SearchResultDto()
            {
                Artists = this._mapper.Map<GuidNameDto[]>(artists.Skip((page - 1) * take).Take(take).ToArray())
            };
        }

        private async Task<SearchResultDto> FilterAllSongs(string searchTerm, int page, int take)
        {
            var songs = this._dbContext.Songs.Where(x => true);

            if (searchTerm != string.Empty)
            {
                songs = songs.Where(x => x.Name.Contains(searchTerm));
            }

            
            return new SearchResultDto()
            {
                Songs = this._mapper.Map<SongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray())
            };
        }

        private async Task<SearchResultDto> FilterAllPlaylists(string searchTerm, int page, int take)
        {
            var playlists = this._dbContext.Playlists.Where(x => true);

            if (searchTerm != string.Empty)
            {
                playlists = playlists.Where(x => x.Name.Contains(searchTerm));
            }
            
            return new SearchResultDto()
            {
                Playlists = this._mapper.Map<PlaylistShortDto[]>(playlists.Skip((page - 1) * take).Take(take).ToArray())
            };
        }

        private async Task<SearchResultDto> FilterAllUsers(string searchTerm, int page, int take)
        {
            var users = this._dbContext.Users.Where(x => true);

            if (searchTerm != string.Empty)
            {
                users = users.Where(x => x.UserName.Contains(searchTerm) || x.Email.Contains(searchTerm));
            }
            
            return new SearchResultDto()
            {
                Users = this._mapper.Map<UserDto[]>(users.Skip((page - 1) * take).Take(take).ToArray())
            };
        }
    }
}
