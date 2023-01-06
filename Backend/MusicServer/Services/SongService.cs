﻿using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
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


            if (!targetPlaylist.IsModifieable || (!sourcePlayList.IsPublic || !canUserSeePlaylist))
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
            var user = this.dBContext.Users
.Include(x => x.Playlists)
.ThenInclude(x => x.Playlist)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var playlist = user.Playlists.FirstOrDefault(x => x.Playlist.Id == playlistId) ?? throw new PlaylistNotFoundException();

            if (!playlist.Playlist.IsPublic)
            {
                throw new NotAllowedException();
            }
        }

        public async Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic)
        {
            var user = this.dBContext.Users
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var entity = this.dBContext.PlaylistUsers.Add(new PlaylistUser()
            {
                IsModifieable= true,
                User = user,
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
                throw new NotAllowedException();
            }

            this.dBContext.Remove(playlist.Playlist);

            await this.dBContext.SaveChangesAsync();
        }

        public Task<List<PlaylistDto>> GetPlaylistsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<PlaylistDto>> GetPublicPlaylists()
        {
            throw new NotImplementedException();
        }

        public Task<List<SongDto>> GetSongsInPlaylist(Guid playlistId)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlaylistDto>> GetUserPlaylists(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveSongsFromPlaylistAsync(Guid playlistId, List<Guid> songIds)
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
                var song = playlist.Playlist.Songs.FirstOrDefault(x => x.Song.Id == songId) ?? throw new SongNotFoundException();

                playlist.Playlist.Songs.Remove(song);
            }

            await this.dBContext.SaveChangesAsync();
        }

        public async Task UpdatePlaylistAsync(Guid playlistId, string name, string description, bool isPublic)
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
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UpdatePlaylistShareListAsync(Guid playlistId, List<UserPlaylistModifieable> dtos)
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

            foreach (var dto in dtos)
            {
                var p = this.dBContext.PlaylistUsers.FirstOrDefault(x => x.Playlist.Id == playlistId && x.User.Id == dto.UserId) ?? throw new PlayListAlreadyInUseException();
                var targetUser = this.dBContext.Users.FirstOrDefault(x => x.Id == dto.UserId) ?? throw new UserNotFoundException();

                this.dBContext.PlaylistUsers.Add(new PlaylistUser()
                {
                    IsModifieable = dto.CanModify,
                    User = targetUser,
                    Playlist = playlist.Playlist
                });
            }

            await this.dBContext.SaveChangesAsync();
        }
    }
}