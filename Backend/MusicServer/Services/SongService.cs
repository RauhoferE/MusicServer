using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.User;
using MusicServer.Entities.Responses;
using MusicServer.Exceptions;
using MusicServer.Helpers;
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

        public async Task<AlbumPaginationResponse> GetAlbumsOfArtist(Guid artistId, int page, int take)
        {
            var artist = this._dbContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            var albums = this._dbContext.ArtistAlbums
                .Include(x => x.Artist)
                .Include(x => x.Album)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Album)
                .ThenInclude(x => x.Songs)
                .Where(x => x.Artist.Id == artistId);

            return new AlbumPaginationResponse()
            {
                Albums = this._mapper.Map<AlbumDto[]>(albums.OrderBy(x => x.Id).Skip((page - 1) * take).Take(take).Select(x => x.Album).ToArray()),
                TotalCount = albums.Count()
            };
        }

        public async Task<ArtistDto> GetArtist(Guid artistId)
        {
            var artist = this._dbContext.Artists
                .Include(x => x.Albums)
                .ThenInclude(x => x.Album)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            var mappedArtists = this._mapper.Map<ArtistDto>(artist);

            var followedArtist = this._dbContext.FollowedArtists
                .Include(x => x.Artist)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Artist.Id == artistId && x.User.Id == this._activeUserService.Id);

            if (followedArtist != null)
            {
                mappedArtists.FollowedByUser = true;
                mappedArtists.ReceiveNotifications = followedArtist.ReceiveNotifications;
            }

            foreach (var song in mappedArtists.Songs)
            {
                var favoriteSong = this._dbContext.FavoriteSongs
                    .Include(x => x.FavoriteSong)
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.FavoriteSong.Id == song.Id && x.User.Id == this._activeUserService.Id);  
                
                if (favoriteSong == null)
                {
                    continue;
                }

                song.IsInFavorites = true;
            }

            return mappedArtists;
        }

        public async Task<SongDto> GetSongInformation(Guid songId)
        {
            var song = this._dbContext.Songs
                .Include(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Album)
                .FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

            var mappedSong = this._mapper.Map<SongDto>(song);

            var favoriteSong = this._dbContext.FavoriteSongs
                .Include(x => x.FavoriteSong)
                .Include(x => x.User)
                .FirstOrDefault(x => x.FavoriteSong.Id == songId && x.User.Id == this._activeUserService.Id);

            if (favoriteSong != null)
            {
                mappedSong.IsInFavorites = true;
            }

            return mappedSong;
        }

        public async Task<SongPaginationResponse> GetSongsInAlbum(Guid albumId, int page, int take)
        {
            var album = this._dbContext.Albums.FirstOrDefault(x => x.Id == albumId) ?? throw new AlbumNotFoundException();

            var songs = this._dbContext.Songs
                .Include(x => x.Artists)
                .ThenInclude(x=> x.Artist)
                .Include(x => x.Album)
                .Where(x => x.Album.Id == albumId);

            var mappedSongs = this._mapper.Map<SongDto[]>(songs.OrderBy(x => x.Id).Skip((page - 1) * take).Take(take));

            foreach (var song in mappedSongs)
            {
                var favoriteSong = this._dbContext.FavoriteSongs
                .Include(x => x.FavoriteSong)
                .Include(x => x.User)
                .FirstOrDefault(x => x.FavoriteSong.Id == song.Id && x.User.Id == this._activeUserService.Id);

                if (favoriteSong == null)
                {
                    continue;
                }

                song.IsInFavorites = true;
            }

            return new SongPaginationResponse()
            {
                Songs = mappedSongs,
                TotalCount = songs.Count()
            };
        }

        public async Task<SearchResultDto> Search(string filter, string searchTerm, int page, int take, string sortAfter, bool asc)
        {
            switch (filter)
            {
                case SearchFilter.All:
                    return await this.FilterAll(searchTerm, page, take);
                case SearchFilter.Albums:
                    return await this.FilterAllAlbums(searchTerm, page, take, sortAfter, asc);
                case SearchFilter.Artists:
                    return await this.FilterAllArtists(searchTerm, page, take, asc);
                case SearchFilter.Songs:
                    return await this.FilterAllSongs(searchTerm, page, take, asc, sortAfter);
                case SearchFilter.Playlists:
                    return await this.FilterAllPlaylists(searchTerm, page, take, asc, sortAfter);
                case SearchFilter.Users:
                    return await this.FilterAllUsers(searchTerm, page, take, asc);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<SearchResultDto> FilterAll(string searchTerm, int page, int take)
        {
            return new SearchResultDto()
            {
                Songs = (await this.FilterAllSongs(searchTerm, page, take, true, "")).Songs,
                Albums = (await this.FilterAllAlbums(searchTerm, page, take, "", true)).Albums,
                Artists = (await this.FilterAllArtists(searchTerm, page, take, true)).Artists,
                Playlists = (await this.FilterAllPlaylists(searchTerm, page, take, true, "")).Playlists,
                Users = (await this.FilterAllUsers(searchTerm, page, take, true)).Users
            };
        }

        private async Task<SearchResultDto> FilterAllAlbums(string searchTerm, int page, int take, string sortAfter, bool asc)
        {
            var albums = this._dbContext.Albums
                .Include(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Songs)
                .Where(x => true);

            albums = SortingHelpers.SortSearchAlbums(albums, asc, sortAfter, searchTerm);

            return new SearchResultDto()
            {
                Albums = this._mapper.Map<AlbumDto[]>(albums.Skip((page - 1) * take).Take(take).ToArray()),
            };
        }

        private async Task<SearchResultDto> FilterAllArtists(string searchTerm, int page, int take, bool asc)
        {
            var artists = this._dbContext.Artists
                .Where(x => true);

            artists = SortingHelpers.SortSearchArtists(artists, asc, searchTerm);

            var mappedArtists = this._mapper.Map<GuidNameDto[]>(artists.Skip((page - 1) * take).Take(take).ToArray());

            foreach (var item in mappedArtists)
            {
                var followedArtist = this._dbContext.FollowedArtists
                    .FirstOrDefault(x => x.Artist.Id == item.Id && x.User.Id == this._activeUserService.Id);

                if (followedArtist == null)
                {
                    continue;
                }

                item.ReceiveNotifications = followedArtist.ReceiveNotifications;
                item.FollowedByUser = true;
            }

            return new SearchResultDto()
            {
                Artists = mappedArtists
            };
        }

        private async Task<SearchResultDto> FilterAllSongs(string searchTerm, int page, int take, bool asc, string sortAfter)
        {
            var songs = this._dbContext.Songs
                                .Include(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Album)
                .Where(x => true);

            songs = SortingHelpers.SortSearchAllSongs(songs, asc, searchTerm, sortAfter);
            var mappedSongs = this._mapper.Map<SongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray());

            foreach (var song in mappedSongs)
            {
                var favorite = this._dbContext.FavoriteSongs
                    .Include(x => x.User)
                    .Include(x => x.FavoriteSong)
                    .FirstOrDefault(x => x.FavoriteSong.Id == song.Id && x.User.Id == this._activeUserService.Id);

                if (favorite == null)
                {
                    continue;
                }

                song.IsInFavorites = true;
            }

            
            return new SearchResultDto()
            {
                Songs = mappedSongs
            };
        }

        private async Task<SearchResultDto> FilterAllPlaylists(string searchTerm, int page, int take, bool asc, string sortAfter)
        {
            var playlists = this._dbContext.Playlists
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Songs)
                .Where(x => x.IsPublic || x.Users.FirstOrDefault(y => y.User.Id == this._activeUserService.Id) != null);

            playlists = SortingHelpers.SortSearchPublicPlaylists(playlists, asc, sortAfter, searchTerm);

            var mappedPlaylists = this._mapper.Map<PlaylistUserShortDto[]>(playlists.Skip((page - 1) * take).Take(take).ToArray());

            foreach (var mappedPlaylist in mappedPlaylists)
            {
                var userPlaylist = this._dbContext.PlaylistUsers.Include(x => x.Playlist).Include(x => x.User)
                    .FirstOrDefault(x => x.User.Id == this._activeUserService.Id && x.Playlist.Id == mappedPlaylist.Id);

                if (userPlaylist == null)
                {
                    mappedPlaylist.IsPublic = true;
                    continue;
                }

                mappedPlaylist.IsPublic = userPlaylist.Playlist.IsPublic;
                mappedPlaylist.ReceiveNotifications = userPlaylist.ReceiveNotifications;
                mappedPlaylist.IsModifieable = userPlaylist.IsModifieable;
                mappedPlaylist.IsCreator = userPlaylist.IsCreator;
                mappedPlaylist.LastListened = userPlaylist.LastListened;
            }


            return new SearchResultDto()
            {
                Playlists = mappedPlaylists
            };
        }

        private async Task<SearchResultDto> FilterAllUsers(string searchTerm, int page, int take, bool asc)
        {
            var users = this._dbContext.Users.Where(x => x.Id != this._activeUserService.Id);

            users = SortingHelpers.SortSearchUsers(users, asc, searchTerm);

            var mappedUsers = this._mapper.Map<UserDto[]>(users.Skip((page - 1) * take).Take(take).ToArray());

            foreach (var user in mappedUsers)
            {
                var followedUser = this._dbContext.FollowedUsers.Include(x => x.User).Include(x => x.FollowedUser)
                    .FirstOrDefault(x => x.User.Id == this._activeUserService.Id && x.FollowedUser.Id == user.Id);

                if (followedUser == null)
                {
                    continue;
                }

                user.IsFollowedByUser = true;
                user.ReceiveNotifications = followedUser.ReceiveNotifications;
            }


            return new SearchResultDto()
            {
                Users = mappedUsers
            };
        }
    }
}
