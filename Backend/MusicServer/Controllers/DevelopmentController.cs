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

        public DevelopmentController(IDevService devService)
        {
            this.devService = devService;
        }

        [HttpGet]
        [Route(ApiRoutes.Development.Test)]
        public async  Task<IActionResult> Test()
        {
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Development.CreateArtistsAndSongs)]
        public async Task<IActionResult> CreateMoqArtistsWithSongs([FromRoute, Required] int artists, [FromRoute, Required] int albums, [FromRoute, Required] int songs)
        {
            await this.devService.AddMoqArtistsAlbumsSongsAsync(artists, albums, songs);
            return NoContent();
        }
    }
}
