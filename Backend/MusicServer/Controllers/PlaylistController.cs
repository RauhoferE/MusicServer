using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Entities.Requests.Song;
using MusicServer.Entities.Requests.User;
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
        [Route(ApiRoutes.Playlist.Default)]
        public async Task<IActionResult> CreatePlaylist([FromBody, Required] UpdatePlaylist request)
        {
            return Ok(await this.playlistService.CreatePlaylistAsync(request.Name, request.Description, request.IsPublic));
        }

        [HttpDelete]
        [Route(ApiRoutes.Playlist.Default)]
        public async Task<IActionResult> DeletePlaylist([FromQuery, Required] Guid playlistId)
        {
            await this.playlistService.DeletePlaylistAsync(playlistId);
            return NoContent();
        }

        [HttpPatch]
        [Route(ApiRoutes.Playlist.Default)]
        public async Task<IActionResult> UpadatePlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] UpdatePlaylist request)
        {
            await this.playlistService.UpdatePlaylistAsync(playlistId, request.Name, request.Description, request.IsPublic);
            return NoContent();
        }

        [HttpPut]
        [Route(ApiRoutes.Playlist.Default)]
        public async Task<IActionResult> AddPlaylistSongsToPlaylist([FromQuery, Required] Guid targetPlaylist, [FromQuery, Required] Guid sourcePlaylist)
        {
            await this.playlistService.AddPlaylistToPlaylistAsync(sourcePlaylist, targetPlaylist);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Default)]
        public async Task<IActionResult> GetPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            var t = await this.playlistService.GetSongsInPlaylist(playlistId, page, take);
            return Ok(t);
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Playlists)]
        public async Task<IActionResult> GetPlaylists([FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            var t = await this.playlistService.GetPlaylistsAsync(page, take);
            return Ok(t);
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.UserPlaylists)]
        public async Task<IActionResult> GetUserPlaylists([FromRoute, Required] Guid userId, [FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            var t = await this.playlistService.GetUserPlaylists(userId, page, take);
            return Ok(t);
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PublicPlaylist)]
        public async Task<IActionResult> GetPublicPlaylists([FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            var t = await this.playlistService.GetPublicPlaylists(page, take);
            return Ok(t);
        }

        [HttpPost]
        [Route(ApiRoutes.Playlist.PlaylistSongs)]
        public async Task<IActionResult> AddSongsToPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.AddSongsToPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Playlist.PlaylistSongs)]
        public async Task<IActionResult> RemoveSongsFromPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.RemoveSongsFromPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PlaylistAlbum)]
        public async Task<IActionResult> AddAlbumToPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid albumId)
        {
            await this.playlistService.AddAlbumSongsToPlaylistAsync(albumId, playlistId);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Playlist.PlaylistShare)]
        public async Task<IActionResult> UpdatePlaylistShareList([FromQuery, Required] Guid playlistId, [FromBody, Required] PlaylistShareList request)
        {
            await this.playlistService.UpdatePlaylistShareListAsync(playlistId, request.UserList.ToList());
            return NoContent();
        }


        [HttpDelete]
        [Route(ApiRoutes.Playlist.PlaylistShare)]
        public async Task<IActionResult> RemoveUserFromPlaylist([FromQuery, Required] Guid playlistId, [FromBody, Required] UserIds request)
        {
            await this.playlistService.RemoveUsersFromPlaylist(playlistId, request.UserIdArray.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PlaylistCopy)]
        public async Task<IActionResult> CopyPlaylistToLibrary([FromQuery, Required] Guid playlistId)
        {
            await this.playlistService.CopyPlaylistToLibraryAsync(playlistId);
            return NoContent();
        }




    }
}
