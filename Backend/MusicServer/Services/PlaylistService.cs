using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;
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

            if (!playlist.IsModifieable)
            {
                throw new NotAllowedException();
            }

            var lastOrderNumber = await this.GetOrderNumberOfLastElementInPlaylist(playlist.Playlist);

            var album = this.dBContext.Albums.Include(x => x.Songs).FirstOrDefault(x => x.Id == albumId) ?? throw new AlbumNotFoundException();

            foreach (var song in album.Songs)
            {
                lastOrderNumber = lastOrderNumber + 1;
                playlist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Order = lastOrderNumber,
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

            var lastOrderNumber = await this.GetOrderNumberOfLastElementInPlaylist(targetPlaylist.Playlist);

            foreach (var song in sourcePlayList.Songs)
            {
                lastOrderNumber = lastOrderNumber + 1;
                targetPlaylist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Order = lastOrderNumber,
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


            var lastSongInOrder = user.Favorites.OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrderNumber = lastSongInOrder == null ? -1 : lastSongInOrder.Order;

            foreach (var songId in songIds)
            {
                var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

                if (!addClones && user.Favorites.FirstOrDefault(x=> x.FavoriteSong.Id == song.Id) != null)
                {
                    doubleSongs.Add(song);
                    continue;
                }
                lastOrderNumber = lastOrderNumber + 1;

                user.Favorites.Add(new UserSong()
                {
                    Order = lastOrderNumber,
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

            var lastOrderNumber = await this.GetOrderNumberOfLastElementInPlaylist(playlist.Playlist);

            foreach (var songId in songIds)
            {
                lastOrderNumber = lastOrderNumber + 1;
                var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

                playlist.Playlist.Songs.Add(new PlaylistSong()
                {
                    Order = lastOrderNumber,
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

        public async Task<PlaylistSongPaginationResponse> GetFavorites(int page, int take)
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

            var songEntities = user.Favorites.Skip((page - 1) * take).Take(take).ToArray();

            var songs = this.mapper.Map<PlaylistSongDto[]>(songEntities);

            return new PlaylistSongPaginationResponse()
            {
                Songs = songs,
                TotalCount = user.Favorites.Count()
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

        public async Task<PlaylistPaginationResponse> GetPlaylistsAsync(long userId, int page, int take)
        {
            var userIdToSearch = userId == -1 ? this.activeUserService.Id : userId;
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
            .FirstOrDefault(x => x.Id == userIdToSearch) ?? throw new UserNotFoundException();

            IEnumerable<PlaylistUser> playlists = user.Playlists;

            if (userIdToSearch > -1)
            {
                playlists = playlists.Where(x => x.Playlist.IsPublic);
            }

            return new PlaylistPaginationResponse()
            {
                Playlists = this.mapper.Map<Playlist[], PlaylistShortDto[]>(user.Playlists
                               .Skip((page - 1) * take)
                                              .Take(take)
                                                             .Select(x => x.Playlist).ToArray()),
                TotalCount = playlists.Count()
            };  
        }

        public async Task<PlaylistPaginationResponse> GetPublicPlaylists(int page, int take)
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

            return new PlaylistPaginationResponse()
            {

                Playlists = this.mapper.Map<Playlist[], PlaylistShortDto[]>(playlists.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = playlists.Count()
            };
        }

        public async Task<PlaylistSongPaginationResponse> GetSongsInPlaylist(Guid playlistId, int page, int take)
        {
            var playlist = this.dBContext.Playlists
                                .Include(x => x.Users)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if ((!playlist.IsPublic && user == null))
            {
                throw new NotAllowedException();
            }

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

            return new PlaylistSongPaginationResponse()
            {
                Songs = this.mapper.Map<PlaylistSong[], PlaylistSongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = songs.Count()
            };
        }


        public async Task RemoveSongsFromFavorite(List<Guid> songIds)
        {
            //TODO: Remove songs from favorite not with the songid but with the order id, because of duplicates
            var user = this.dBContext.Users.Include(x => x.Favorites)
    .ThenInclude(x => x.FavoriteSong).FirstOrDefault(x => x.Id == this.activeUserService.Id)
    ?? throw new UserNotFoundException();

            foreach (var songId in songIds)
            {
                var favoriteSong = user.Favorites.FirstOrDefault(x => x.FavoriteSong.Id == songId) ?? throw new SongNotFoundException();

                user.Favorites.Remove(favoriteSong);
            }

            // Fix the order of the songs
            foreach (var item in user.Favorites)
            {
                if (item.Order == 0)
                {
                    continue;
                }

                var itemWhereOrderIsSmaller = user.Favorites.OrderBy(x => x.Order).LastOrDefault(x => x.Order < item.Order);

                if (itemWhereOrderIsSmaller != null && itemWhereOrderIsSmaller.Order == item.Order - 1)
                {
                    continue;
                }

                item.Order = itemWhereOrderIsSmaller == null ? 0 : itemWhereOrderIsSmaller.Order + 1;
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

            // Fix the order of the songs
            foreach (var item in playlist.Playlist.Songs)
            {
                if (item.Order == 0)
                {
                    continue;
                }

                var itemWhereOrderIsSmaller = playlist.Playlist.Songs.OrderBy(x => x.Order).LastOrDefault(x => x.Order < item.Order);

                if (itemWhereOrderIsSmaller != null && itemWhereOrderIsSmaller.Order == item.Order - 1)
                {
                    continue;
                }

                item.Order = itemWhereOrderIsSmaller == null ? 0 : itemWhereOrderIsSmaller.Order + 1;
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

        public async Task<PlaylistSongPaginationResponse> SearchSongInFavorites(string query, int page, int take)
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

            var allSongEntities = user.Favorites
                .Where(x => x.FavoriteSong.Name.Contains(query) ||
                        x.FavoriteSong.Artists.Where(x => x.Artist.Name.Contains(query)).Any());

            var songs = this.mapper.Map<PlaylistSongDto[]>(allSongEntities.Skip((page - 1) * take).Take(take).ToArray());

            return new PlaylistSongPaginationResponse()
            {
                Songs = songs,
                TotalCount = allSongEntities.Count()
            };
        }

        public async Task<PlaylistSongPaginationResponse> SearchSongInPlaylist(Guid playlistId, string query, int page, int take)
        {
            var playlist = this.dBContext.Playlists
                .Include(x => x.Users)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if ((!playlist.IsPublic && user == null))
            {
                throw new NotAllowedException();
            }

            var songs = this.dBContext.PlaylistSongs
                .Include(x => x.Playlist)
                .ThenInclude(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Song)
                .ThenInclude(x => x.Album)
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Where(x => x.Playlist.Id == playlistId && 
                (x.Song.Name.ToLower().Contains(query) || 
                    x.Song.Artists.Where(x => x.Artist.Name.Contains(query)).Any()));

            return new PlaylistSongPaginationResponse()
            {
                Songs = this.mapper.Map<PlaylistSong[], PlaylistSongDto[]>(songs.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = songs.Count()
            };
        }

        public async Task<PlaylistPaginationResponse> SearchUserPlaylist(long userId, string query, int page, int take)
        {
            var userIdToSearch = userId == -1 ? this.activeUserService.Id : userId;
            User user = user = this.dBContext.Users
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
                .FirstOrDefault(x => x.Id == userIdToSearch) ?? throw new UserNotFoundException(); 

            IEnumerable<PlaylistUser> playlists = user.Playlists;
            
            if (userId > -1)
            {
                playlists = user.Playlists.Where(x => x.Playlist.IsPublic);
            }

            playlists = playlists.Where(x => x.Playlist.Name.ToLower().Contains(query.ToLower()));

            return new PlaylistPaginationResponse()
            {
                Playlists = this.mapper.Map<Playlist[], PlaylistShortDto[]>(playlists
               .Skip((page - 1) * take)
                              .Take(take)
                                             .Select(x => x.Playlist).ToArray()),
                TotalCount = playlists.Count()
            };
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

        public Task ChangeOrderSpotOfFavorit(Guid songId, int newSpot)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOrderSpotOfSongInPlaylist(Guid playlistId, Guid songId, int newSpot)
        {
            throw new NotImplementedException();
        }

        public Task ChangeOrderOfPlaylist(Guid playlistId, int newSpot)
        {
            throw new NotImplementedException();
        }

        private async Task<int> GetOrderNumberOfLastElementInPlaylist(Playlist playlist)
        {
            var lastSongInOrder = playlist.Songs.OrderByDescending(x => x.Order).FirstOrDefault();

            if (lastSongInOrder == null)
            {
                return -1;
            }

            return lastSongInOrder.Order;
        }
    }
}
