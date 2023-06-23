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
            return Ok(await this.playlistService.CreatePlaylistAsync(request.Name, request.Description, request.IsPublic, request.ReceiveNotifications));
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
            await this.playlistService.UpdatePlaylistAsync(playlistId, request.Name, request.Description, request.IsPublic, request.ReceiveNotifications);
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
        public async Task<IActionResult> GetPlaylistInfo([FromQuery, Required] Guid playlistId)
        {
            var t = await this.playlistService.GetPlaylistInfo(playlistId);
            return Ok(t);
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Songs)]
        public async Task<IActionResult> GetSongsInPlaylist([FromRoute, Required] Guid playlistId, [FromQuery, Required] int page, [FromQuery, Required] int take, [FromQuery] string query = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(await this.playlistService.GetSongsInPlaylist(playlistId, page, take));
            }
            
            return Ok(await this.playlistService.SearchSongInPlaylist(playlistId, query, page, take));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Playlists)]
        public async Task<IActionResult> GetPlaylists([FromQuery, Required] int page, [FromQuery, Required] int take, [FromQuery] long userId = -1, [FromQuery] string query = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(await this.playlistService.GetPlaylistsAsync(userId, page, take));
            }
            
            return Ok(await this.playlistService.SearchUserPlaylist(userId, query, page, take));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PublicPlaylist)]
        public async Task<IActionResult> GetPublicPlaylists([FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            var t = await this.playlistService.GetPublicPlaylists(page, take);
            return Ok(t);
        }

        [HttpPost]
        [Route(ApiRoutes.Playlist.Songs)]
        public async Task<IActionResult> AddSongsToPlaylist([FromRoute, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.AddSongsToPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Playlist.Songs)]
        public async Task<IActionResult> RemoveSongsFromPlaylist([FromRoute, Required] Guid playlistId, [FromBody, Required] SongsToPlaylist request)
        {
            await this.playlistService.RemoveSongsFromPlaylistAsync(playlistId, request.SongIds.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PlaylistAlbum)]
        public async Task<IActionResult> AddAlbumToPlaylist([FromRoute, Required] Guid playlistId, [FromQuery, Required] Guid albumId)
        {
            await this.playlistService.AddAlbumSongsToPlaylistAsync(albumId, playlistId);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Playlist.PlaylistShare)]
        public async Task<IActionResult> UpdatePlaylistShareList([FromRoute, Required] Guid playlistId, [FromBody, Required] PlaylistShareList request)
        {
            await this.playlistService.UpdatePlaylistShareListAsync(playlistId, request.UserList.ToList());
            return NoContent();
        }


        [HttpDelete]
        [Route(ApiRoutes.Playlist.PlaylistShare)]
        public async Task<IActionResult> RemoveUserFromPlaylist([FromRoute, Required] Guid playlistId, [FromBody, Required] UserIds request)
        {
            await this.playlistService.RemoveUsersFromPlaylist(playlistId, request.UserIdArray.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PlaylistCopy)]
        public async Task<IActionResult> CopyPlaylistToLibrary([FromRoute, Required] Guid playlistId)
        {
            await this.playlistService.CopyPlaylistToLibraryAsync(playlistId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PlaylistAddToLibrary)]
        public async Task<IActionResult> AddPlaylistToLibrary([FromRoute, Required] Guid playlistId)
        {
            await this.playlistService.AddPlaylistToLibraryAsync(playlistId);
            return NoContent();
        }

        [HttpPost]
        [Route(ApiRoutes.Playlist.Favorite)]
        public async Task<IActionResult> AddSongToFavorites([FromBody, Required] SongsToPlaylist songs, [FromQuery, Required] bool addClones)
        {
            await this.playlistService.AddSongsToFavorite(songs.SongIds.ToList(), addClones);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.Playlist.Favorite)]
        public async Task<IActionResult> RemoveSongsFromFavorites([FromBody, Required] SongsToPlaylist songs)
        {
            await this.playlistService.RemoveSongsFromFavorite(songs.SongIds.ToList());
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Favorite)]
        public async Task<IActionResult> GetFavorites([FromQuery, Required] int page, [FromQuery, Required] int take, [FromQuery] string query = null)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(await this.playlistService.GetFavorites(page, take));
            }
            
            return Ok(await this.playlistService.SearchSongInFavorites(query, page, take));
        }


    }
}
