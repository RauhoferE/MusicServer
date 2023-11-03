using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Entities.Requests.Multi;
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
        public async Task<IActionResult> GetSongsInPlaylist([FromRoute, Required] Guid playlistId, [FromQuery, Required] QueryPaginationSearchRequest request)
        {
            return Ok(await this.playlistService.GetSongsInPlaylist(playlistId, request.Page, request.Take, request.SortAfter, request.Asc, request.Query));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.Playlists)]
        public async Task<IActionResult> GetPlaylists([FromQuery, Required] QueryPaginationSearchRequest request, [FromQuery] long userId = -1)
        {
            return Ok(await this.playlistService.GetPlaylistsAsync(userId, request.Page, request.Take, request.SortAfter, request.Asc, request.Query));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.ModifieablePlaylists)]
        public async Task<IActionResult> GetModifieablePlaylists([FromQuery] long userId = -1)
        {
            return Ok(await this.playlistService.GetModifiablePlaylists(userId));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.PublicPlaylist)]
        public async Task<IActionResult> GetPublicPlaylists([FromQuery, Required] QueryPaginationSearchRequest request)
        {
            var t = await this.playlistService.GetPublicPlaylists(request.Page, request.Take, request.SortAfter, request.Asc, request.Query);
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
        public async Task<IActionResult> RemoveSongsFromPlaylist([FromRoute, Required] Guid playlistId, [FromBody, Required] SongsToRemove request)
        {
            await this.playlistService.RemoveSongsFromPlaylistAsync(playlistId, request.OrderIds.ToList());
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
        public async Task<IActionResult> AddSongToFavorites([FromBody, Required] SongsToPlaylist songs)
        {
            await this.playlistService.AddSongsToFavorite(songs.SongIds.ToList());
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
        public async Task<IActionResult> GetFavorites([FromQuery, Required] QueryPaginationSearchRequest request)
        {
            return Ok(await this.playlistService.GetFavorites(request.Page, request.Take, request.SortAfter, request.Asc, request.Query));
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.OrderFavorites)]
        public async Task<IActionResult> ChangeOrderOfFavorit([FromQuery, Required] Guid songId, [FromQuery, Required] int order)
        {
            await this.playlistService.ChangeOrderOfFavorit(songId, order);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.OrderSongs)]
        public async Task<IActionResult> ChangeOrderOfSongInPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] Guid songId, [FromQuery, Required] int order)
        {
            await this.playlistService.ChangeOrderOfSongInPlaylist(playlistId, songId, order);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.Playlist.OrderPlaylists)]
        public async Task<IActionResult> ChangeOrderOfPlaylist([FromQuery, Required] Guid playlistId, [FromQuery, Required] int order)
        {
            await this.playlistService.ChangeOrderOfPlaylist(playlistId, order);
            return NoContent();
        }


    }
}
