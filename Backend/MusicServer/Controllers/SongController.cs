using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Controllers
{
    [Authorize]
    public class SongController : Controller
    {
        private readonly ISongService songService;

        public SongController(ISongService songService)
        {
            this.songService= songService;
        }

        [HttpGet]
        [Route(ApiRoutes.Song.Artist)]
        public async Task<IActionResult> GetArtistInfo([FromRoute, Required] Guid artistId)
        {
            return Ok(await this.songService.GetArtist(artistId));
        }

        [HttpGet]
        [Route(ApiRoutes.Song.ArtistAlbums)]
        public async Task<IActionResult> GetAlbumsOfArtists([FromRoute, Required] Guid artistId, [FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            return Ok(await this.songService.GetAlbumsOfArtist(artistId, take, page));
        }

        [HttpGet]
        [Route(ApiRoutes.Song.SongsInAlbum)]
        public async Task<IActionResult> GetSongsInAlbum([FromRoute, Required] Guid albumId, [FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            return Ok(await this.songService.GetSongsInAlbum(albumId, page, take));
        }

        [HttpGet]
        [Route(ApiRoutes.Song.SongDefault)]
        public async Task<IActionResult> GetSongInfo([FromRoute, Required] Guid songId)
        {
            return Ok(await this.songService.GetSongInformation(songId));
        }

        [HttpGet]
        [Route(ApiRoutes.Song.Search)]
        public async Task<IActionResult> Search([FromQuery, Required] int page, [FromQuery, Required] int take, [FromBody, Required] Search request)
        {
            return Ok(await this.songService.Search(request.Filter, request.SearchTerm, page, take));
        }
    }
}
