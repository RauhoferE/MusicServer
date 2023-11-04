using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
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

        //[HttpGet(ApiRoutes.File.Song)]
        //public async Task<IActionResult> Song([FromRoute, Required] Guid songId)
        //{
        //    Response.ContentType = "audio/mp3"; 
        //    return File(await this.fileService.GetSongStream(songId), "audio/mp3");
        //}

        [HttpGet(ApiRoutes.File.Album)]
        public async Task<IActionResult> Album([FromRoute, Required] Guid albumId)
        {
            Response.ContentType = "image/jpeg";
            return File(await this.fileService.GetAlbumCover(albumId), "image/jpeg");
        }

        [HttpGet(ApiRoutes.File.Artist)]
        public async Task<IActionResult> Artist([FromRoute, Required] Guid artistId)
        {
            Response.ContentType = "image/jpeg";
            return File(await this.fileService.GetArtistCover(artistId), "image/jpeg");
        }

        [HttpGet(ApiRoutes.File.Playlist)]
        public async Task<IActionResult> Playlist([FromRoute, Required] Guid playlistId)
        {
            Response.ContentType = "image/png";
            return File(await this.fileService.GetPlaylistCover(playlistId), "image/png");
        }

        [HttpGet(ApiRoutes.File.User)]
        public async Task<IActionResult> User([FromRoute, Required] long userId)
        {
            Response.ContentType = "image/png";
            return File(await this.fileService.GetUserAvatar(userId), "image/png");
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

                await this.fileService.UploadPlaylistCover(playlistId, file, Path.GetExtension(file.FileName));
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
                await this.fileService.UploadUserAvatar(file, Path.GetExtension(file.FileName));
                return Ok();
            }

            return BadRequest();
        }
    }
}
