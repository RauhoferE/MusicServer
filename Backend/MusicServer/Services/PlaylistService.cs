﻿using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Responses;
using MusicServer.Exceptions;
using MusicServer.Helpers;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly MusicServerDBContext dBContext;

        private readonly IActiveUserService activeUserService;

        private readonly IMapper mapper;

        private readonly IAutomatedMessagingService automatedMessagingService;

        public PlaylistService(IActiveUserService activeUserService,
            MusicServerDBContext dBContext,
            IAutomatedMessagingService automatedMessagingService,
            IMapper mapper)
        {
            this.activeUserService = activeUserService;
            this.dBContext = dBContext;
            this.automatedMessagingService = automatedMessagingService;
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

            var lastOrderNumber = await this.GetOrderNumberOfLastPlaylist(user);   

            user.Playlists.Add(new PlaylistUser()
            {
                IsCreator = false,
                IsModifieable = false,
                Playlist = playlist,
                User = user,
                Order = lastOrderNumber + 1
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

            var targetPlaylist = user.Playlists
                .FirstOrDefault(x => x.Playlist.Id == targetPlaylistId) ?? throw new PlaylistNotFoundException();
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

        public async Task AddSongsToFavoriteAsync(List<Guid> songIds)
        {
            var user = this.dBContext.Users.Include(x => x.Favorites)
                .ThenInclude(x => x.FavoriteSong).FirstOrDefault(x => x.Id == this.activeUserService.Id) 
                ?? throw new UserNotFoundException();

            var lastSongInOrder = user.Favorites.OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrderNumber = lastSongInOrder == null ? -1 : lastSongInOrder.Order;

            foreach (var songId in songIds)
            {
                var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();

                if (user.Favorites.FirstOrDefault(x=> x.FavoriteSong.Id == song.Id) != null)
                {
                    // Skip if song is already in favorites
                    continue;
                }

                lastOrderNumber = lastOrderNumber + 1;

                user.Favorites.Add(new UserSong()
                {
                    Order = lastOrderNumber,
                    FavoriteSong = song
                });
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

            await this.automatedMessagingService.AddSongsToPlaylistMessageAsync(user.Id, playlist.Playlist.Id, songIds.Distinct().ToList());

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
                .Include(x => x.Playlists)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var lastOrderNumber = await this.GetOrderNumberOfLastPlaylist(user);

            var entity = this.dBContext.PlaylistUsers.Add(new PlaylistUser()
            {
                IsModifieable= true,
                User = user,
                IsCreator=true,
                ReceiveNotifications =receiveNotifications,
                Order = lastOrderNumber + 1,
                Playlist = new Playlist()
                {
                    Description= description,
                    IsPublic= isPublic,
                    Name= name,
                    Songs = new List<PlaylistSong>()                    
                }
            });

            await this.dBContext.SaveChangesAsync();

            if (isPublic)
            {
                await this.automatedMessagingService.AddPlaylistMessageAsync(this.activeUserService.Id, entity.Entity.Playlist.Id);
            }

            return entity.Entity.Playlist.Id;
        }

        public async Task DeletePlaylistAsync(Guid playlistId)
        {
            var user = this.dBContext.Users
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var allUserPlaylists = this.dBContext.PlaylistUsers
                .Include(x => x.Playlist)
                .Include(x => x.User)
                .Where(x => x.User.Id == this.activeUserService.Id && x.Order > playlist.Order);

            foreach (var pl in allUserPlaylists)
            {
                pl.Order--;
            }

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

        public async Task<PlaylistSongPaginationResponse> GetFavoritesAsync(int skip, int take, string sortAfter, bool asc, string query)
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

            var songentities = SortingHelpers.SortSearchFavorites(user.Favorites.AsQueryable(), asc, sortAfter, query);

            var songs = this.mapper.Map<PlaylistSongDto[]>(songentities.Skip(skip).Take(take).ToArray());

            foreach (var song in songs)
            {
                song.IsInFavorites = true;
            }

            return new PlaylistSongPaginationResponse()
            {
                Songs = songs,
                TotalCount = songentities.Count()
            };
        }

        public async Task<PlaylistSongPaginationResponse> GetFavoritesOfUserAsync(long userId, int skip, int take, string sortAfter, bool asc, string query)
        {
            var user = this.dBContext.Users.Include(x => x.Favorites)
.ThenInclude(x => x.FavoriteSong)
.ThenInclude(x => x.Album)
.Include(x => x.Favorites)
.ThenInclude(x => x.FavoriteSong)
                .ThenInclude(x => x.Artists)
                    .ThenInclude(x => x.Artist)
.FirstOrDefault(x => x.Id == userId)
?? throw new UserNotFoundException();

            var songentities = SortingHelpers.SortSearchFavorites(user.Favorites.AsQueryable(), asc, sortAfter, query);

            var songs = this.mapper.Map<PlaylistSongDto[]>(songentities.Skip(skip).Take(take).ToArray());

            foreach (var song in songs)
            {
                song.IsInFavorites = true;
            }

            return new PlaylistSongPaginationResponse()
            {
                Songs = songs,
                TotalCount = songentities.Count()
            };
        }

        public async Task<PlaylistUserShortDto> GetPlaylistInfoAsync(Guid playlistId)
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

            var mappedPlaylist = this.mapper.Map<PlaylistUserShortDto>(playlist);

            if (user != null)
            {
                mappedPlaylist.IsCreator = user.IsCreator;
                mappedPlaylist.IsModifieable = user.IsModifieable;
                mappedPlaylist.ReceiveNotifications = user.ReceiveNotifications;
                mappedPlaylist.LastListened = user.LastListened;
            }

            return mappedPlaylist;
        }

        public async Task<PlaylistPaginationResponse> GetPlaylistsAsync(long userId, int page, int take, string sortAfter, bool asc, string query)
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

            // add sorting here
            IEnumerable<PlaylistUser> playlists = SortingHelpers.SortSearchUserPlaylists(user.Playlists.AsQueryable(), asc, sortAfter, query);
            //IEnumerable<PlaylistUser> playlists = user.Playlists;

            List<PlaylistUserShortDto> userPlaylists = new List<PlaylistUserShortDto>();
            if (userId > -1)
            {
                var activeUser = this.dBContext.Users
                    .Include(x => x.Playlists)
                    .ThenInclude(x => x.Playlist)
                    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

                playlists = playlists.Where(x => x.Playlist.IsPublic ||
                x.Playlist.Users.FirstOrDefault(y => y.User.Id == this.activeUserService.Id) != null);

                playlists = SortingHelpers.SortSearchUserPlaylists(playlists.AsQueryable(), asc, sortAfter, query);

                // Add sorting here

                foreach (var playlist in playlists.Skip((page - 1) * take)
                                              .Take(take))
                {
                    var activeUserPlaylist = activeUser.Playlists.FirstOrDefault(x => x.Playlist.Id == playlist.Playlist.Id);
                    if (activeUserPlaylist != null)
                    {
                        var mappedPlaylist = this.mapper.Map<PlaylistUserShortDto>(activeUserPlaylist);
                        userPlaylists.Add(mappedPlaylist);
                        continue;
                    }

                    var mappedPlaylist2 = this.mapper.Map<PlaylistUserShortDto>(playlist);
                    mappedPlaylist2.IsModifieable = false;
                    mappedPlaylist2.ReceiveNotifications = false;
                    mappedPlaylist2.IsCreator = false;
                    userPlaylists.Add(mappedPlaylist2);
                }

                return new PlaylistPaginationResponse()
                {
                    Playlists = userPlaylists.ToArray(),
                    TotalCount = playlists.Count()
                };
            }



            return new PlaylistPaginationResponse()
            {
                Playlists = this.mapper.Map<PlaylistUserShortDto[]>(playlists.Skip((page - 1) * take)
                                              .Take(take).ToArray()),
                TotalCount = playlists.Count()
            };  
        }

        public async Task<PlaylistPaginationResponse> GetPublicPlaylistsAsync(int page, int take, string sortAfter, bool asc, string query)
        {
            var playlists = this.dBContext.Playlists
                .Include(x => x.Songs)
                .ThenInclude(x => x.Song)
                .Include(x => x.Users)
                .ThenInclude(x => x.User)
                .Include(x => x.Songs)
                .Where(x => x.IsPublic);

            playlists = SortingHelpers.SortSearchPublicPlaylists(playlists, asc, sortAfter, query);

            List<PlaylistUserShortDto> userPlaylists = new List<PlaylistUserShortDto>();

            var activeUser = this.dBContext.Users
    .Include(x => x.Playlists)
    .ThenInclude(x => x.Playlist)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            foreach (var playlist in playlists.Skip((page - 1) * take)
                              .Take(take))
            {
                var activeUserPlaylist = activeUser.Playlists.FirstOrDefault(x => x.Playlist.Id == playlist.Id);
                if (activeUserPlaylist != null)
                {
                    var mappedPlaylist = this.mapper.Map<PlaylistUserShortDto>(activeUserPlaylist);
                    userPlaylists.Add(mappedPlaylist);
                    continue;
                }

                var mappedPlaylist2 = this.mapper.Map<PlaylistUserShortDto>(playlist);
                mappedPlaylist2.IsModifieable = false;
                mappedPlaylist2.ReceiveNotifications = false;
                mappedPlaylist2.IsPublic = true;
                mappedPlaylist2.IsCreator = false;
                userPlaylists.Add(mappedPlaylist2);
            }

            return new PlaylistPaginationResponse()
            {

                Playlists = userPlaylists.ToArray(),
                TotalCount = playlists.Count()
            };
        }

        public async Task<PlaylistSongPaginationResponse> GetSongsInPlaylistAsync(Guid playlistId, int skip, int take, string sortAfter, bool asc, string query)
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

            songs = SortingHelpers.SortSearchSongsInPlaylist(songs, asc, sortAfter, query);

            var mappedSongs = this.mapper.Map<PlaylistSong[], PlaylistSongDto[]>(songs.Skip(skip).Take(take).ToArray());

            foreach (var song in mappedSongs)
            {
                var followedSong = this.dBContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong)
                    .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.FavoriteSong.Id == song.Id);

                if (followedSong == null)
                {
                    continue;
                }

                song.IsInFavorites = true;
            }

            return new PlaylistSongPaginationResponse()
            {
                Songs = mappedSongs,
                TotalCount = songs.Count()
            };
        }

        public async Task<PlaylistSongPaginationResponse> GetSongsInPlaylistOfUserAsync(long userId, Guid playlistId, int skip, int take, string sortAfter, bool asc, string query)
        {
            var playlist = this.dBContext.Playlists
                    .Include(x => x.Users)
        .ThenInclude(x => x.User)
    .FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == userId);

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

            songs = SortingHelpers.SortSearchSongsInPlaylist(songs, asc, sortAfter, query);

            var mappedSongs = this.mapper.Map<PlaylistSong[], PlaylistSongDto[]>(songs.Skip(skip).Take(take).ToArray());

            foreach (var song in mappedSongs)
            {
                var followedSong = this.dBContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong)
                    .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.FavoriteSong.Id == song.Id);

                if (followedSong == null)
                {
                    continue;
                }

                song.IsInFavorites = true;
            }

            return new PlaylistSongPaginationResponse()
            {
                Songs = mappedSongs,
                TotalCount = songs.Count()
            };
        }

        public async Task RemoveSongsFromFavoriteAsync(List<Guid> songIds)
        {
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

        public async Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<int> orderIds)
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

            foreach (var songId in orderIds)
            {
                var song = playlist.Playlist.Songs.FirstOrDefault(x => x.Order == songId) ?? throw new SongNotFoundException();

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

        public async Task RemoveUsersFromPlaylistAsync(Guid playlistId, List<long> userIds)
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

                var allUserPlaylists = this.dBContext.PlaylistUsers
                    .Include(x => x.Playlist)
                    .Include(x => x.User)
                    .Where(x => x.User.Id == userId && x.Order > entityToRemove.Order);

                foreach (var pl in allUserPlaylists)
                {
                    pl.Order--;
                }

                this.dBContext.Remove(entityToRemove);

                await this.automatedMessagingService.PlaylistRemoveMessageAsync(this.activeUserService.Id, playlist.Playlist.Id, entityToRemove.User.Id);
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
                var targetUser = this.dBContext.Users.Include(x => x.Playlists).ThenInclude(x => x.Playlist)
                    .FirstOrDefault(x => x.Id == dto.UserId) ?? throw new UserNotFoundException();

                if (p != null && p.IsCreator)
                {
                    throw new NotAllowedException();
                }

                if (p != null)
                {
                    p.IsModifieable = dto.CanModify;
                    continue;
                }

                var lastOrderNumber = await this.GetOrderNumberOfLastPlaylist(targetUser);

                this.dBContext.PlaylistUsers.Add(new PlaylistUser()
                {
                    IsModifieable = dto.CanModify,
                    IsCreator = false,
                    User = targetUser,
                    Playlist = playlist.Playlist,
                    Order = lastOrderNumber + 1
                });

                await this.automatedMessagingService
                    .PlaylistShareMessageAsync(this.activeUserService.Id, playlist.Playlist.Id, targetUser.Id);
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task ChangeOrderOfFavoritAsync(int oldSpot, int newSpot)
        {
            var favorites = this.dBContext.Users
                .Include(x => x.Favorites)
                .ThenInclude(x => x.FavoriteSong)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id)?
                .Favorites
                ?? throw new UserNotFoundException();

            var songToMove = favorites.FirstOrDefault(x => x.Order == oldSpot)
                ?? throw new SongNotFoundException();

            var targetPlace = favorites.FirstOrDefault(x => x.Order == newSpot) 
                ?? throw new SongNotFoundException();

            var oldSongOrder = songToMove.Order;    
            songToMove.Order = newSpot;

            var favoritesToTraverse = favorites.Where(x => x.Order <= newSpot && x.FavoriteSong.Id != songToMove.FavoriteSong.Id && x.Order > oldSongOrder);

            if (oldSongOrder > newSpot)
            {
                favoritesToTraverse = favorites.Where(x => x.Order >= newSpot && x.FavoriteSong.Id != songToMove.FavoriteSong.Id && x.Order < oldSongOrder);
            }

            foreach (var songBefore in favoritesToTraverse)
            {
                if (oldSongOrder >= newSpot)
                {
                    songBefore.Order++;
                    continue;
                }

                songBefore.Order--;
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task ChangeOrderOfSongInPlaylistAsync(Guid playlistId, int oldSpot, int newSpot)
        {
            var playlist = this.dBContext.PlaylistUsers
                .Include(x => x.Playlist)
                .ThenInclude(x => x.Songs)
                .ThenInclude(x => x.Song)
                .Include(x => x.User)
                .FirstOrDefault(x => x.Playlist.Id == playlistId && x.User.Id == this.activeUserService.Id)
                ?? throw new PlaylistNotFoundException();

            var songToMove = playlist.Playlist.Songs
                .FirstOrDefault(x => x.Order == oldSpot)
                ?? throw new SongNotFoundException();

            var targetPlace = playlist.Playlist.Songs
                .FirstOrDefault(x => x.Order == newSpot)
                ?? throw new SongNotFoundException();

            var oldSongOrder = songToMove.Order;
            songToMove.Order = newSpot;

            var songsToTraverse = playlist.Playlist.Songs.Where(x => x.Order <= newSpot && x.Song.Id != songToMove.Song.Id && x.Order > oldSongOrder);

            if (oldSongOrder > newSpot)
            {
                songsToTraverse = playlist.Playlist.Songs.Where(x => x.Order >= newSpot && x.Song.Id != songToMove.Song.Id && x.Order < oldSongOrder);
            }

            foreach (var songBefore in songsToTraverse)
            {
                if (oldSongOrder >= newSpot)
                {
                    songBefore.Order++;
                    continue;
                }

                songBefore.Order--;
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task ChangeOrderOfPlaylistAsync(Guid playlistId, int newSpot)
        {
            var playlists = this.dBContext.PlaylistUsers
    .Include(x => x.Playlist)
    .Include(x => x.User)
    .Where(x => x.User.Id == this.activeUserService.Id)
    ?? throw new PlaylistNotFoundException();

            var playlistToMove = playlists
                .FirstOrDefault(x => x.Playlist.Id == playlistId)
                ?? throw new SongNotFoundException();

            var targetPlace = playlists
                .FirstOrDefault(x => x.Order == newSpot)
                ?? throw new SongNotFoundException();


            var oldPlaylistOrder = playlistToMove.Order;
            playlistToMove.Order = newSpot;

            var playlistsToTraverse = playlists.Where(x => x.Order <= newSpot && x.Playlist.Id != playlistId && x.Order > oldPlaylistOrder);

            if (oldPlaylistOrder > newSpot)
            {
                playlistsToTraverse = playlists.Where(x => x.Order >= newSpot && x.Playlist.Id != playlistId && x.Order < oldPlaylistOrder);
            }

            foreach (var playlistBefore in playlistsToTraverse)
            {
                if (oldPlaylistOrder >= newSpot)
                {
                    playlistBefore.Order++;
                    continue;
                }

                playlistBefore.Order--;
            }

            await this.dBContext.SaveChangesAsync();
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

        private async Task<int> GetOrderNumberOfLastPlaylist(User user)
        {
            var lastSongInOrder = user.Playlists.OrderByDescending(x => x.Order).FirstOrDefault();

            if (lastSongInOrder == null)
            {
                return -1;
            }

            return lastSongInOrder.Order;
        }

        public async Task<ModifieablePlaylistsResponse> GetModifiablePlaylistsAsync(long userId)
        {
            var userIdToSearch = userId == -1 ? this.activeUserService.Id : userId;
            var playlists = this.dBContext.PlaylistUsers
                .Include(x => x.Playlist)
                .Include(x => x.User)
            .Where(x => x.User.Id == userIdToSearch && x.IsModifieable) ?? throw new UserNotFoundException();

                return new ModifieablePlaylistsResponse()
                {
                    Playlists = this.mapper.Map<GuidNameDto[]>(playlists)
                };
        }

        public async Task<int> GetPlaylistSongCountAsync(Guid playlistId)
        {
            var p = this.dBContext.Playlists.FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            return this.dBContext.PlaylistSongs.Include(x => x.Playlist).Count(x => x.Playlist.Id == playlistId);
        }

        public async Task<int> GetFavoriteSongCountAsync()
        {
            var userId = this.activeUserService.Id;
            return this.dBContext.FavoriteSongs.Count(x => x.User.Id == userId);
        }

        public async Task<int> GetFavoriteSongCountAsync(long userId)
        {
            return this.dBContext.FavoriteSongs.Count(x => x.User.Id == userId);
        }

        public async Task SetNotifications(Guid playlistId)
        {
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId);

            if (playlist == null)
            {
                await this.AddPlaylistToLibraryAsync(playlistId);
                user = this.dBContext.Users
                    .Include(x => x.Playlists)
                    .ThenInclude(x => x.Playlist)
                    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();
                playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId);
            }

            playlist.ReceiveNotifications = !playlist.ReceiveNotifications;
            await this.dBContext.SaveChangesAsync();
        }
    }
}
