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

            var guid = await this.CreatePlaylistAsync(playlist.Name, playlist.Description, playlist.IsPublic);

            await this.AddSongsToPlaylistAsync(guid, playlist.Songs.Select(x => x.Song.Id).ToList());
        }

        public async Task<Guid> CreatePlaylistAsync(string name, string description, bool isPublic)
        {
            var user = this.dBContext.Users
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var entity = this.dBContext.PlaylistUsers.Add(new PlaylistUser()
            {
                IsModifieable= true,
                User = user,
                IsCreator=true,
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

        public async Task<List<PlaylistDto>> GetPlaylistsAsync()
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

            return this.mapper.Map<List<Playlist>, List<PlaylistDto>>(user.Playlists.Select(x => x.Playlist).ToList());
        }

        public async Task<List<PlaylistDto>> GetPublicPlaylists()
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

            return this.mapper.Map<List<Playlist>, List<PlaylistDto>>(playlists.ToList());
        }

        public async Task<PlaylistDto> GetSongsInPlaylist(Guid playlistId)
        {
            var playlist = this.dBContext.Playlists
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
            .FirstOrDefault(x => x.Id == playlistId) ?? throw new PlaylistNotFoundException();

            var user = playlist.Users.FirstOrDefault(x => x.User.Id == this.activeUserService.Id);

            if ((!playlist.IsPublic && user == null))
            {
                throw new NotAllowedException();
            }

            return this.mapper.Map<Playlist, PlaylistDto>(playlist);
        }

        public async Task<List<PlaylistDto>> GetUserPlaylists(Guid userId)
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

            return this.mapper.Map<List<Playlist>, List<PlaylistDto>>(user.Playlists.Where(x => x.Playlist.IsPublic).Select(x => x.Playlist).ToList());
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

        public async Task RemoveUsersFromPlaylist(Guid playlistId, List<Guid> userIds)
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
