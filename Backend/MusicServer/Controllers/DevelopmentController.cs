using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Core.Const;
using MusicServer.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize(Roles = "Root")]
    public class DevelopmentController : Controller
    {
        private readonly IDevService devService;
        private readonly IMusicMailService mailService;

        public DevelopmentController(IDevService devService, IMusicMailService mailService)
        {
            this.devService = devService;
            this.mailService = mailService; 
        }

        //[HttpGet]
        //[Route(ApiRoutes.Development.Test)]
        //public async  Task<IActionResult> Test()
        //{
        //    await this.mailService.SendEmail(null, "Test", "Test");
        //    return Ok();
        //}

        [HttpGet]
        [Route(ApiRoutes.Development.CreateArtistsAndSongs)]
        public async Task<IActionResult> CreateMoqArtistsWithSongs([FromRoute, Required] int artists, [FromRoute, Required] int albums, [FromRoute, Required] int songs)
        {
            await this.devService.AddMoqArtistsAlbumsSongsAsync(artists, albums, songs);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Development.CreateUsersAndPlaylists)]
        public async Task<IActionResult> CreateMoqUsersWithPlaylists([FromRoute, Required] int users, [FromRoute, Required] int playlists)
        {
            await this.devService.AddMoqUsersAndPlaylists(users, playlists);
            return NoContent();
        }
    }
}
