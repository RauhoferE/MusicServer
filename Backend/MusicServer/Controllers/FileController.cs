using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Serilog;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFileService fileService;

        public FileController(IFileService fileService)
        {
                this.fileService = fileService;
        }

        [HttpGet(ApiRoutes.File.Song)]
        public async Task<IActionResult> Song([FromRoute, Required] Guid songId)
        {
            Response.ContentType = "audio/mp3";
            var f =  File(await this.fileService.GetSongStreamAsync(songId), "audio/mp3");
            f.EnableRangeProcessing = true;
            return f;
        }

        [HttpGet(ApiRoutes.File.Album)]
        public async Task<IActionResult> Album([FromRoute, Required] Guid albumId)
        {
            Response.ContentType = "image/jpeg";
            return File(await this.fileService.GetAlbumCoverAsync(albumId), "image/jpeg");
        }

        [HttpGet(ApiRoutes.File.Artist)]
        public async Task<IActionResult> Artist([FromRoute, Required] Guid artistId)
        {
            Response.ContentType = "image/jpeg";
            return File(await this.fileService.GetArtistCoverAsync(artistId), "image/jpeg");
        }

        [HttpGet(ApiRoutes.File.Playlist)]
        public async Task<IActionResult> Playlist([FromRoute, Required] Guid playlistId)
        {
            Response.ContentType = "image/png";
            return File(await this.fileService.GetPlaylistCoverAsync(playlistId), "image/png");
        }

        [HttpGet(ApiRoutes.File.User)]
        public async Task<IActionResult> User([FromRoute, Required] long userId)
        {
            Response.ContentType = "image/png";
            return File(await this.fileService.GetUserAvatarAsync(userId), "image/png");
        }

        [HttpPost(ApiRoutes.File.Playlist)]
        public async Task<IActionResult> PlaylistCoverUpload([FromRoute, Required] Guid playlistId, IFormFile file)
        {
            if (file != null && file.Length > 0 && file.Length <= 1000000)
            {
                if (Path.GetExtension(file.FileName) != ".png")
                {
                    throw new UploadedFileNotSupportedException();
                }

                await this.fileService.UploadPlaylistCoverAsync(playlistId, file, Path.GetExtension(file.FileName));
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost(ApiRoutes.File.OwnUser)]
        public async Task<IActionResult> UserCoverUpload(IFormFile file)
        {
            if (file != null && file.Length > 0 && file.Length <= 1000000)
            {
                if (Path.GetExtension(file.FileName) != ".png")
                {
                    throw new UploadedFileNotSupportedException();
                }
                await this.fileService.UploadUserAvatarAsync(file, Path.GetExtension(file.FileName));
                return Ok();
            }

            return BadRequest();
        }
    }
}
