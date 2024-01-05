using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using MusicServer.Settings;
using System.Collections;
using static MusicServer.Const.ApiRoutes;

namespace MusicServer.Services
{
    public class FileService : IFileService
    {
        private readonly IActiveUserService activeUserService;
        private readonly MusicServerDBContext dBContext;
        private readonly ISftpService sftpService;
        private readonly FileserverSettings fileserverSettings;

        public FileService(IActiveUserService activeUserService, 
            MusicServerDBContext dBContext, ISftpService sftpService, IOptions<FileserverSettings> fileSettings)
        {
            this.activeUserService = activeUserService;
            this.dBContext = dBContext;
            this.sftpService = sftpService;
            this.fileserverSettings = fileSettings.Value;
        }

        public async Task<byte[]> GetAlbumCoverAsync(Guid albumId)
        {
            var album = this.dBContext.Albums.FirstOrDefault(x => x.Id == albumId)
                ?? throw new AlbumNotFoundException();

            var path = $"{this.fileserverSettings.AlbumCoverFolder}/{albumId}.jpg";
            if (!(await this.sftpService.FileExistsAsync(path)))
            {
                return await this.GetLocalFile(Path.Combine("Assets\\Images\\No-Album-Cover.png"));
            }


            return await this.sftpService.DownloadFileAsync(path);
        }

        public async Task<byte[]> GetArtistCoverAsync(Guid artistId)
        {
            var artist = this.dBContext.Artists
                .Include(x => x.Albums)
                .ThenInclude(x => x.Album)
                .FirstOrDefault(x => x.Id == artistId)
    ?? throw new ArtistNotFoundException();

            var latestAlbum = artist.Albums.OrderByDescending(x => x.Album.Release).FirstOrDefault()
                ?? throw new AlbumNotFoundException();

            
            return await this.GetAlbumCoverAsync(latestAlbum.Album.Id);
        }

        public async Task<byte[]> GetPlaylistCoverAsync(Guid playlistId)
        {
            var playlistUser = this.dBContext.PlaylistUsers
                .Include(x => x.User)
                .Include(x => x.Playlist)
                .FirstOrDefault(x => (x.User.Id == this.activeUserService.Id && x.Playlist.Id == playlistId) ||
                x.Playlist.IsPublic && x.Playlist.Id == playlistId)
                ?? throw new NotAllowedException();

            var path = $"{this.fileserverSettings.PlaylistCoverFolder}/{playlistId}.png";
            if (!(await this.sftpService.FileExistsAsync(path)))
            {
                return await this.GetLocalFile(Path.Combine("Assets\\Images\\No-Playlist-Cover.png"));
            }

            return await this.sftpService.DownloadFileAsync(path);
        }

        public async Task<byte[]> GetSongStreamAsync(Guid songId)
        {
            var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId)
                ?? throw new SongNotFoundException();

            var path = $"{this.fileserverSettings.SongFolder}/{songId}.mp3";// Path.Combine(this.fileserverSettings.SongFolder, $"{songId}.mp3");
            if (!( await this.sftpService.FileExistsAsync(path)))
            {
                throw new FileNotFoundException();
            }

            return await this.sftpService.DownloadFileAsync(path);
        }

        public async Task<byte[]> GetUserAvatarAsync(long userId)
        {
            if (userId == -1)
            {
                userId = this.activeUserService.Id;
            }

            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId)
                ?? throw new UserNotFoundException();

            var path = $"{this.fileserverSettings.ProfileCoverFolder}/{userId}.png";
            if (!(await this.sftpService.FileExistsAsync(path)))
            {
                return await this.GetLocalFile(Path.Combine("Assets\\Images\\No-Profile-Cover.png"));
            }

            return await this.sftpService.DownloadFileAsync(path);
        }

        public async Task UploadPlaylistCoverAsync(Guid playlistId, IFormFile image, string extension)
        {
            var playlistUser = this.dBContext.PlaylistUsers
                .Include(x => x.User)
                .Include(x => x.Playlist)
                .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.Playlist.Id == playlistId)
                ?? throw new NotAllowedException();

            var path = $"{this.fileserverSettings.PlaylistCoverFolder}/{playlistId}{extension}";

            using (var stream = image.OpenReadStream())
            {
                if (await sftpService.FileExistsAsync(path))
                {
                    await sftpService.DeleteFileAsync(path);
                }

                await this.sftpService.UploadFileAsync(stream, path);
            }
        }

        public async Task UploadUserAvatarAsync(IFormFile image, string extension)
        {
            var path = $"{this.fileserverSettings.ProfileCoverFolder}/{this.activeUserService.Id}{extension}";

            using (var stream = image.OpenReadStream())
            {
                if (await sftpService.FileExistsAsync(path))
                {
                    await sftpService.DeleteFileAsync(path);
                }

                await this.sftpService.UploadFileAsync(stream, path);
            }
        }

        private async Task<byte[]> GetLocalFile(string path)
        {
            var byteArray = new byte[1024];

            using (var stream = System.IO.File.OpenRead(path))
            {
                byteArray = new byte[stream.Length];
                stream.Read(byteArray, 0, byteArray.Length);
            }

            return byteArray;
        }
    }
}
