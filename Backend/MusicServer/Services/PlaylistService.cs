using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly MusicServerDBContext dBContext;

        private readonly IActiveUserService activeUserService;

        private readonly IMapper mapper;

        public PlaylistService(IActiveUserService activeUserService,
            MusicServerDBContext dBContext,
            IMapper mapper)
        {
            this.activeUserService = activeUserService;
            this.dBContext = dBContext;
            this.mapper = mapper;
        }

        public async Task AddAlbumSongsToPlaylistAsync(Guid albumId, Guid playlistId)
        {
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.ThenInclude(x => x.Songs)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();
            var album = this.dBContext.Albums.Include(x => x.Songs).FirstOrDefault(x => x.Id == albumId) ?? throw new AlbumNotFoundException();

            if (!playlist.IsModifieable)
            {
                throw new NotAllowedException();
            }

            foreach (var song in album.Songs)
            {
                playlist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Song= song
                });
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddPlaylistToLibraryAsync(Guid playlistId)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = this.dBContext.Playlists.FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) != null)
            {
                throw new AlreadyAssignedException();
            }

            if (!playlist.IsPublic)
            {
                throw new NotAllowedException();
            }

            user.Playlists.Add(new PlaylistUser()
            {
                IsCreator = false,
                IsModifieable = false,
                Playlist = playlist,
                User = user
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddPlaylistToPlaylistAsync(Guid sourcePlaylistId, Guid targetPlaylistId)
        {
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.ThenInclude(x => x.Songs)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var targetPlaylist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == targetPlaylistId) ?? throw new PlaylistNotFoundException();
            var sourcePlayList = this.dBContext.Playlists
                .Include(x => x.Songs).ThenInclude(x => x.Song)
                .FirstOrDefault(x => x.Id == sourcePlaylistId) ?? throw new PlaylistNotFoundException();

            var canUserSeePlaylist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == sourcePlaylistId) == null ? false : true;


            if (!targetPlaylist.IsModifieable || (!sourcePlayList.IsPublic && !canUserSeePlaylist))
            {
                throw new NotAllowedException();
            }

            foreach (var song in sourcePlayList.Songs)
            {
                targetPlaylist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Song = song.Song
                });
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddSongsToFavorite(List<Guid> songIds, bool addClones)
        {
            var user = this.dBContext.Users.Include(x => x.Favorites)
                .ThenInclude(x => x.FavoriteSong).FirstOrDefault(x => x.Id == this.activeUserService.Id) 
                ?? throw new UserNotFoundException();

            var doubleSongs = new List<Song>();

            foreach (var songId in songIds)
            {
                var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

                if (!addClones && user.Favorites.FirstOrDefault(x=> x.FavoriteSong.Id == song.Id) != null)
                {
                    doubleSongs.Add(song);
                    continue;
                }

                user.Favorites.Add(new UserSong()
                {
                    FavoriteSong = song
                });
            }

            if (doubleSongs.Any())
            {
                throw new AlreadyAssignedException(string.Join(",", doubleSongs.Select(x => x.Name)));
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddSongsToPlaylistAsync(Guid playlistId, List<Guid> songIds)
        {
            var user = this.dBContext.Users
    .Include(x => x.Playlists)
    .ThenInclude(x => x.Playlist)
    .ThenInclude(x => x.Songs)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable)
            {
                throw new NotAllowedException();
            }

            foreach (var songId in songIds)
            {
                var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

                playlist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Song= song
                });
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task CopyPlaylistToLibraryAsync(Guid playlistId)
        {
            var playlist = this.dBContext.Playlists
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if (!playlist.IsPublic && user == null)
            {
                throw new NotAllowedException();
            }

            var guid = await this.CreatePlaylistAsync(playlist.Name, playlist.Description, playlist.IsPublic, true);

            await this.AddSongsToPlaylistAsync(guid, playlist.Songs.Select(x => x.Song.Id).ToList());
        }

        public async Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic, bool receiveNotifications)
        {
            var user = this.dBContext.Users
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var entity = this.dBContext.PlaylistUsers.Add(new PlaylistUser()
            {
                IsModifieable= true,
                User = user,
                IsCreator=true,
                ReceiveNotifications =receiveNotifications,
                Playlist = new Playlist()
                {
                    Description= description,
                    IsPublic= isPublic,
                    Name= name,
                    Songs = new List<PlaylistSong>()                    
                }
            });

            await this.dBContext.SaveChangesAsync();
            return entity.Entity.Playlist.Id;
        }

        public async Task DeletePlaylistAsync(Guid playlistId)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable)
            {
                var userPlaylist = this.dBContext.PlaylistUsers
                    .Include(x => x.User)
                    .Include(x => x.Playlist)
                    .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.Playlist.Id == playlistId)
                    ?? throw new NotAllowedException();

                this.dBContext.PlaylistUsers.Remove(userPlaylist);
                await this.dBContext.SaveChangesAsync();
                return;
            }

            this.dBContext.Remove(playlist.Playlist);

            await this.dBContext.SaveChangesAsync();
        }

        public async Task<FavoriteDto> GetFavorites(int page, int take)
        {
            var user = this.dBContext.Users.Include(x => x.Favorites)
            .ThenInclude(x => x.FavoriteSong)
            .ThenInclude(x => x.Album)
            .Include(x => x.Favorites)
            .ThenInclude(x => x.FavoriteSong)
                            .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
            .FirstOrDefault(x => x.Id == this.activeUserService.Id)
            ?? throw new UserNotFoundException();

            var songEntities = user.Favorites.Skip((page - 1) * take).Take(take).ToArray().Select(x => x.FavoriteSong).ToArray();

            var songs = this.mapper.Map<SongDto[]>(songEntities);

            return new FavoriteDto()
            {
                Songs = songs,
                SongCount = user.Favorites.Count()
            };
        }

        public async Task<PlaylistDto> GetPlaylistInfo(Guid playlistId)
        {
            var playlist = this.dBContext.Playlists
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .FirstOrDefault(x => x.Id == playlistId) ?? throw  new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if (user == null && !playlist.IsPublic)
            {
                throw new NotAllowedException();
            }

            return this.mapper.Map<PlaylistDto>(playlist);
        }

        public async Task<PlaylistShortDto[]> GetPlaylistsAsync(int page, int take)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Album)
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
            .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            return this.mapper.Map<Playlist[], PlaylistShortDto[]>(user.Playlists
                .Skip((page - 1) * take)
                .Take(take)
                .Select(x => x.Playlist).ToArray());
        }

        public async Task<PlaylistShortDto[]> GetPublicPlaylists(int page, int take)
        {
            var playlists = this.dBContext.Playlists
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Album)
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Where(x => x.IsPublic);

            return this.mapper.Map<Playlist[], PlaylistShortDto[]>(playlists.Skip((page - 1) * take).Take(take).ToArray());
        }

        public async Task<SongDto[]> GetSongsInPlaylist(Guid playlistId, int page, int take)
        {
            var playlist = this.dBContext.Playlists.FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();
            var songs = this.dBContext.PlaylistSongs
                .Include(x => x.Playlist)
                .ThenInclude(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Song)
                .ThenInclude(x => x.Album)
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Where(x => x.Playlist.Id == playlistId);


            var user = songs.First().Playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if ((!songs.First().Playlist.IsPublic && user == null))
            {
                throw new NotAllowedException();
            }

            return this.mapper.Map<PlaylistSong[], SongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray());
        }

        public async Task<PlaylistShortDto[]> GetUserPlaylists(long userId, int page, int take)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Album)
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
.FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            return this.mapper.Map<Playlist[], PlaylistShortDto[]>(
                user.Playlists.Where(x => x.Playlist.IsPublic)
                .Skip((page - 1) * take).Take(take).Select(x => x.Playlist).ToArray());
        }

        public async Task RemoveSongsFromFavorite(List<Guid> songIds)
        {
            var user = this.dBContext.Users.Include(x => x.Favorites)
    .ThenInclude(x => x.FavoriteSong).FirstOrDefault(x => x.Id == this.activeUserService.Id)
    ?? throw new UserNotFoundException();

            foreach (var songId in songIds)
            {
                var favoriteSong = user.Favorites.FirstOrDefault(x => x.FavoriteSong.Id == songId) ?? throw new SongNotFoundException();

                user.Favorites.Remove(favoriteSong);
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds)
        {
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.ThenInclude(x => x.Songs)
.ThenInclude(x => x.Song)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable)
            {
                throw new NotAllowedException();
            }

            foreach (var songId in songIds)
            {
                var song = playlist.Playlist.Songs.FirstOrDefault(x => x.Song.Id == songId) ?? throw new SongNotFoundException();

                playlist.Playlist.Songs.Remove(song);
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task RemoveUsersFromPlaylist(Guid playlistId, List<long> userIds)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                //.Include(x => x.Playlists)
                //.ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable || !playlist.IsCreator)
            {
                throw new NotAllowedException();
            }

            var playlistEntity = this.dBContext.PlaylistUsers
                .Include(x => x.Playlist)
                .Include(x => x.User)
                .Where(x => x.Playlist.Id == playlistId);

            foreach (var userId in userIds)
            {
                if (userId == this.activeUserService.Id)
                {
                    continue;
                }

                var entityToRemove = playlistEntity.FirstOrDefault(x => x.User.Id == userId) ?? throw new UserNotFoundException();

                this.dBContext.Remove(entityToRemove);
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic, bool receiveNotifications)
        {
            var user = this.dBContext.Users
    .Include(x => x.Playlists)
    .ThenInclude(x => x.Playlist)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable)
            {
                throw new NotAllowedException();
            }

            playlist.Playlist.Name = name;
            playlist.Playlist.Description= description;
            playlist.Playlist.IsPublic = isPublic;
            playlist.ReceiveNotifications = receiveNotifications;
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos)
        {
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.IsModifieable || !playlist.IsCreator)
            {
                throw new NotAllowedException();
            }

            foreach (var dto in dtos)
            {
                var p = this.dBContext.PlaylistUsers.FirstOrDefault(x => x.Playlist.Id == playlistId && x.User.Id == dto.UserId);
                var targetUser = this.dBContext.Users.FirstOrDefault(x => x.Id == dto.UserId) ?? throw new UserNotFoundException();

                if (p.IsCreator)
                {
                    throw new NotAllowedException();
                }

                if (p != null)
                {
                    p.IsModifieable = dto.CanModify;
                    continue;
                }

                this.dBContext.PlaylistUsers.Add(new PlaylistUser()
                {
                    IsModifieable = dto.CanModify,
                    IsCreator = false,
                    User = targetUser,
                    Playlist = playlist.Playlist
                });
            }

            await this.dBContext.SaveChangesAsync();
        }
    }
}
