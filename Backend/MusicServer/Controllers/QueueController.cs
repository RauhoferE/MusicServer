using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Core.Const;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using System.ComponentModel.DataAnnotations;
using static MusicServer.Const.ApiRoutes;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class QueueController : Controller
    {
        private readonly IQueueService queueService;

        private readonly IPlaylistService playlistService;

        private readonly ISongService songService;

        public QueueController(IQueueService queueService, IPlaylistService playlistService, ISongService songService)
        {
            this.queueService = queueService;
            this.playlistService = playlistService;
            this.songService = songService;
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueAlbum)]
        public async Task<IActionResult> CreateQueueFromAlbum(Guid albumId, [FromQuery, Required] bool randomize, [FromQuery, Required] string loopMode, [FromQuery]int playFromIndex = 0)
        {
            var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(albumId);
            var albumSongs = await this.songService.GetSongsInAlbumAsync(albumId, 0, albumSongCount);
            await this.queueService.UpdateQueueDataAsync(albumId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Album, randomize, true);
            return Ok(await this.queueService.CreateQueueAsync(albumSongs.Songs, randomize, playFromIndex));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueuePlaylist)]
        public async Task<IActionResult> CreateQueueFromPlaylist(Guid playlistId, [FromQuery, Required] bool randomize, [FromQuery, Required] string loopMode, [FromQuery] string sortAfter = null, [FromQuery] bool asc = true, [FromQuery] int playFromOrder = 0)
        {
            var playlistSongCount = await this.playlistService.GetPlaylistSongCountAsync(playlistId);
            var playlistSongs = await this.playlistService.GetSongsInPlaylistAsync(playlistId, 0, playlistSongCount, sortAfter, asc, null);
            await this.queueService.UpdateQueueDataAsync(playlistId, loopMode, sortAfter, QueueTarget.Playlist, randomize, asc);
            return Ok(await this.queueService.CreateQueueAsync(playlistSongs.Songs, randomize, playFromOrder));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueFavorites)]
        public async Task<IActionResult> CreateQueueFromFavorites([FromQuery, Required] bool randomize, [FromQuery, Required] string loopMode, [FromQuery] string sortAfter = null, [FromQuery] bool asc = true, [FromQuery] int playFromOrder = 0)
        {
            var favoriteSongCount = await this.playlistService.GetFavoriteSongCountAsync();
            var favoriteSongs = await this.playlistService.GetFavoritesAsync(0, favoriteSongCount, sortAfter, asc, null);
            await this.queueService.UpdateQueueDataAsync(Guid.Empty, loopMode, sortAfter, QueueTarget.Favorites, randomize, asc);
            return Ok(await this.queueService.CreateQueueAsync(favoriteSongs.Songs, randomize, playFromOrder));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueSingleSong)]
        public async Task<IActionResult> CreateQueueFromSingleSong(Guid songId, [FromQuery, Required] bool randomize, [FromQuery, Required] string loopMode)
        {
            var song = await this.songService.GetSongInformationAsync(songId);
            await this.queueService.UpdateQueueDataAsync(songId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Song, randomize, true);

            if (randomize)
            {
                var albumCount = await this.songService.GetSongCountOfAlbumAsync(song.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(song.Album.Id, 0, albumCount);
                albumSongs.Songs = albumSongs.Songs.Where(x => x.Id != songId).Prepend(song).ToArray();
                return Ok(await this.queueService.CreateQueueAsync(albumSongs.Songs, randomize, 0));
            }

            return Ok(await this.queueService.CreateQueueAsync(new[] {song} , false, -1));

        }

        [HttpGet]
        [Route(ApiRoutes.Queue.Default)]
        public async Task<IActionResult> GetCurrentQueue()
        {
            return Ok(await this.queueService.GetCurrentQueueAsync());
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SongWithIndex)]
        public async Task<IActionResult> GetSongWithIndexFromQueue([FromRoute, Required] int index)
        {
            return Ok(await this.queueService.GetSongInQueueWithIndexAsync(index));
        }

        [HttpDelete]
        [Route(ApiRoutes.Queue.Default)]
        public async Task<IActionResult> ClearCurrentQueue()
        {
            await this.queueService.ClearQueueAsync();
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CurrentSong)]
        public async Task<IActionResult> GetCurrentSong()
        {
            return Ok(await this.queueService.GetCurrentSongInQueueAsync());
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SkipForward)]
        public async Task<IActionResult> SkipForwardInQueue([FromQuery]int index = 0)
        {
            if (index < 1)
            {
                return Ok(await this.queueService.SkipForwardInQueueAsync());
            }

            return Ok(await this.queueService.SkipForwardInQueueAsync(index));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SkipBack)]
        public async Task<IActionResult> SkipBackInQueue()
        {
            return Ok(await this.queueService.SkipBackInQueueAsync());
        }

        [HttpPost]
        [Route(ApiRoutes.Queue.RemoveSongsFromQueue)]
        public async Task<IActionResult> RemoveSongsInQueue([FromBody, Required] SongsToRemove request)
        {
            return Ok(await this.queueService.RemoveSongsWithIndexFromQueueAsync(request.OrderIds));
        }

        [HttpPost]
        [Route(ApiRoutes.Queue.AddSongsToQueue)]
        public async Task<IActionResult> AddSongsInQueue([FromBody, Required] SongsToPlaylist request)
        {
            await this.queueService.AddSongsToQueueAsync(request.SongIds);
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.PushSongInQueue)]
        public async Task<IActionResult> PushSongsInQueue([FromQuery, Required] int srcIndex, [FromQuery, Required] int targetIndex)
        {
            return Ok(await this.queueService.PushSongToIndexAsync(srcIndex, targetIndex));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.RandomizeQueue)]
        public async Task<IActionResult> RandomizeQueue([FromQuery]Guid playlistId, [FromQuery] Guid albumId, [FromQuery] Guid songId, [FromQuery, Required] string loopMode)
        {
            if (playlistId == Guid.Empty && albumId == Guid.Empty && songId == Guid.Empty)
            {
                // Get favroites
                var favoriteSongCount = await this.playlistService.GetFavoriteSongCountAsync();
                var favorites = await this.playlistService.GetFavoritesAsync(0, favoriteSongCount, null, true, null);
                await this.queueService.UpdateQueueDataAsync(Guid.Empty, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Favorites, true, true);
                return Ok(await this.queueService.RandomizeQueueAsync(favorites.Songs));
            }

            if (playlistId != Guid.Empty)
            {
                // Get playlist
                var playlistSongCount = await this.playlistService.GetPlaylistSongCountAsync(playlistId);
                var playlistSongs = await this.playlistService.GetSongsInPlaylistAsync(playlistId, 0, playlistSongCount, null, true, null);
                await this.queueService.UpdateQueueDataAsync(playlistId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Playlist, true, true);
                return Ok(await this.queueService.RandomizeQueueAsync(playlistSongs.Songs));
            }

            if (albumId != Guid.Empty)
            {
                // Get Album songs
                var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(albumId);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(albumId, 0, albumSongCount);
                await this.queueService.UpdateQueueDataAsync(albumId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Album, true, true);
                return Ok(await this.queueService.RandomizeQueueAsync(albumSongs.Songs));
            }

            if (songId != Guid.Empty)
            {
                var songDetails = await this.songService.GetSongInformationAsync(songId);
                var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(songDetails.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(songDetails.Album.Id, 0, albumSongCount);
                await this.queueService.UpdateQueueDataAsync(songId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Song, true, true);
                return Ok(await this.queueService.RandomizeQueueAsync(albumSongs.Songs));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.QueueData)]
        public async Task<IActionResult> GetQueueData()
        {
            return Ok(await this.queueService.GetQueueDataAsync());
        }

        [HttpPost]
        [Route(ApiRoutes.Queue.QueueData)]
        public async Task<IActionResult> UpdateQueueData([FromQuery, Required]Guid itemId, [FromQuery, Required] string loopMode, [FromQuery, Required] string sortAfter, [FromQuery, Required] string target, [FromQuery, Required] bool randomize, [FromQuery, Required] bool asc)
        {
            await this.queueService.UpdateQueueDataAsync(itemId, loopMode, sortAfter, target, randomize, asc);
            return Ok(await this.queueService.GetQueueDataAsync());
        }


    }
}
