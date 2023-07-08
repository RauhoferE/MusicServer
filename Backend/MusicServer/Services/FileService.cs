using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using MusicServer.Settings;
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

        public async Task<byte[]> GetAlbumCover(Guid albumId)
        {
            var album = this.dBContext.Albums.FirstOrDefault(x => x.Id == albumId)
                ?? throw new AlbumNotFoundException();

            var path = Path.Combine(this.fileserverSettings.AlbumCoverFolder,
                $"{albumId}.jpg");
            if (!(await this.sftpService.FileExists(path)))
            {
                throw new FileNotFoundException();
            }


            return await this.sftpService.DownloadFile(path);
        }

        public async Task<byte[]> GetArtistCover(Guid artistId)
        {
            var artist = this.dBContext.Artists
                .Include(x => x.Albums)
                .ThenInclude(x => x.Album)
                .FirstOrDefault(x => x.Id == artistId)
    ?? throw new ArtistNotFoundException();

            var latestAlbum = artist.Albums.OrderByDescending(x => x.Album.Release).FirstOrDefault()
                ?? throw new AlbumNotFoundException();

            
            return await this.GetAlbumCover(latestAlbum.Album.Id);
        }

        public async Task<byte[]> GetPlaylistCover(Guid playlistId)
        {
            var playlistUser = this.dBContext.PlaylistUsers
                .Include(x => x.User)
                .Include(x => x.Playlist)
                .FirstOrDefault(x => (x.User.Id == this.activeUserService.Id && x.Playlist.Id == playlistId) ||
                x.Playlist.IsPublic && x.Playlist.Id == playlistId)
                ?? throw new NotAllowedException();

            var path = Path.Combine(this.fileserverSettings.PlaylistCoverFolder, $"{playlistId}.png");
            if (!(await this.sftpService.FileExists(path)))
            {
                throw new FileNotFoundException();
            }

            return await this.sftpService.DownloadFile(path);
        }

        public async Task<byte[]> GetSongStream(Guid songId)
        {
            var song = this.dBContext.Songs.FirstOrDefault(x => x.Id == songId)
                ?? throw new SongNotFoundException();

            var path = Path.Combine(this.fileserverSettings.SongFolder, $"{songId}.mp3");
            if (!( await this.sftpService.FileExists(path)))
            {
                throw new FileNotFoundException();
            }

            return await this.sftpService.DownloadFile(path);
        }

        public async Task<byte[]> GetUserAvatar(long userId)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId)
                ?? throw new UserNotFoundException();

            var path = Path.Combine(this.fileserverSettings.ProfileCoverFolder, $"{userId}.png");
            if (!(await this.sftpService.FileExists(path)))
            {
                throw new FileNotFoundException();
            }

            return await this.sftpService.DownloadFile(path);
        }

        public async Task UploadPlaylistCover(Guid playlistId, IFormFile image, string extension)
        {
            var playlistUser = this.dBContext.PlaylistUsers
                .Include(x => x.User)
                .Include(x => x.Playlist)
                .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.Playlist.Id == playlistId)
                ?? throw new NotAllowedException();

            var path = Path.Combine(this.fileserverSettings.PlaylistCoverFolder, $"{playlistId}{extension}");

            using (var stream = image.OpenReadStream())
            {
                if (await sftpService.FileExists(path))
                {
                    await sftpService.DeleteFile(path);
                }

                await this.sftpService.UploadFile(stream, path);
            }
        }

        public async Task UploadUserAvatar(IFormFile image, string extension)
        {
            var path = Path.Combine(this.fileserverSettings.ProfileCoverFolder,
                $"{this.activeUserService.Id}{extension}");
            using (var stream = image.OpenReadStream())
            {
                if (await sftpService.FileExists(path))
                {
                    await sftpService.DeleteFile(path);
                }

                await this.sftpService.UploadFile(stream, path);
            }
        }
    }
}
