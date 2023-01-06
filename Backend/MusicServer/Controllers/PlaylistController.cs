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
    public class PlaylistController : Controller
    {
        private readonly IPlaylistService playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            this.playlistService = playlistService;
        }

        [HttpPost]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> CreatePlaylist([FromBody, Required] UpdatePlaylist request)
        {
            return Ok(await this.playlistService.CreatePlaylistAsync(request.Name, request.Description, request.IsPublic));
        }

        [HttpDelete]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> DeletePlaylist([FromQuery, Required] Guid playlistId)
        {
            await this.playlistService.DeletePlaylistAsync(playlistId);
            return NoContent();
        }

        [HttpPatch]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> UpadatePlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] UpdatePlaylist request)
        {
            await this.playlistService.UpdatePlaylistAsync(playlistId, request.Name, request.Description, request.IsPublic);
            return NoContent();
        }

        [HttpPut]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> AddPlaylistSongsToPlaylist([FromQuery, Required] Guid targetPlaylist, [FromQuery, Required] Guid sourcePlaylist)
        {
            await this.playlistService.AddPlaylistToPlaylistAsync(sourcePlaylist, targetPlaylist);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.Playlist)]
        public async Task<IActionResult> GetPlaylist([FromQuery, Required] Guid playlistId)
        {
            var t = await this.playlistService.GetSongsInPlaylist(playlistId);
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.Playlists)]
        public async Task<IActionResult> GetPlaylists()
        {
            var t = await this.playlistService.GetPlaylistsAsync();
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.UserPlaylists)]
        public async Task<IActionResult> GetUserPlaylists([FromRoute, Required] Guid userId)
        {
            var t = await this.playlistService.GetUserPlaylists(userId);
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.PublicPlaylist)]
        public async Task<IActionResult> GetPublicPlaylists()
        {
            var t = await this.playlistService.GetPublicPlaylists();
            return Ok();
        }

        [HttpPost]
        [Route(ApiRoutes.Song.PlaylistSongs)]
        public async Task<IActionResult> AddSongsToPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.AddSongsToPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Song.PlaylistSongs)]
        public async Task<IActionResult> RemoveSongsFromPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.RemoveSongsFromPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.PlaylistAlbum)]
        public async Task<IActionResult> AddAlbumToPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid albumId)
        {
            await this.playlistService.AddAlbumSongsToPlaylistAsync(playlistId, albumId);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Song.PlaylistShare)]
        public async Task<IActionResult> UpdatePlaylistShareList([FromQuery, Required] Guid playlistId, [FromBody, Required] PlaylistShareList request)
        {
            await this.playlistService.UpdatePlaylistShareListAsync(playlistId, request.UserList.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Song.PlaylistCopy)]
        public async Task<IActionResult> CopyPlaylistToLibrary([FromQuery, Required] Guid playlistId)
        {
            await this.playlistService.CopyPlaylistToLibraryAsync(playlistId);
            return NoContent();
        }




    }
}
