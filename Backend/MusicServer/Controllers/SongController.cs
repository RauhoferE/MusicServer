using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class SongController : Controller
    {
        private readonly ISongService songService;

        public SongController(ISongService songService)
        {
            this.songService = songService;
        }

        [HttpPost]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> CreatePlaylist([FromBody, Required] UpdatePlaylist request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> DeletePlaylist([FromQuery, Required] Guid playlistId)
        {
            throw new NotImplementedException();
        }

        [HttpPatch]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> UpadatePlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] UpdatePlaylist request)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(ApiRoutes.Song.PlaylistSongs)]
        public async Task<IActionResult> AddSongsToPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        [Route(ApiRoutes.Song.PlaylistSongs)]
        public async Task<IActionResult> RemoveSongsFromPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.PlaylistAlbum)]
        public async Task<IActionResult> AddAlbumToPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid albumId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> AddPlaylistSongsToPlaylist([FromQuery, Required] Guid targetPlaylist, [FromQuery, Required] Guid sourcePlaylist)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route(ApiRoutes.Song.PlaylistShare)]
        public async Task<IActionResult> UpdatePlaylistShareList([FromQuery, Required] Guid playlistId, [FromBody, Required] PlaylistShareList request)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.PlaylistCopy)]
        public async Task<IActionResult> CopyPlaylistToLibrary([FromQuery, Required] Guid playlistId)
        {
            throw new NotImplementedException();
        }


    }
}
