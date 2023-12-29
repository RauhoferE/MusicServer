using DataAccess.Entities;
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

        // TODO: Add action for only playing one song

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueAlbum)]
        public async Task<IActionResult> CreateQueueFromAlbum(Guid albumId, [FromQuery, Required] bool randomize, [FromQuery]int playFromIndex = 0)
        {
            var albumSongCount = await this.songService.GetSongCountOfAlbum(albumId);
            var albumSongs = await this.songService.GetSongsInAlbum(albumId, playFromIndex, albumSongCount);
            return Ok(await this.queueService.CreateQueue(albumSongs.Songs, randomize));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueuePlaylist)]
        public async Task<IActionResult> CreateQueueFromPlaylist(Guid playlistId, [FromQuery, Required] bool randomize, [FromQuery, Required] string sortAfter = null, [FromQuery, Required] bool asc = true, [FromQuery] int playFromIndex = 0)
        {
            var playlistSongCount = await this.playlistService.GetPlaylistSongCount(playlistId);
            var playlistSongs = await this.playlistService.GetSongsInPlaylist(playlistId, playFromIndex, playlistSongCount, sortAfter, asc, null);
            return Ok(await this.queueService.CreateQueue(playlistSongs.Songs, randomize));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueFavorites)]
        public async Task<IActionResult> CreateQueueFromFavorites([FromQuery, Required] bool randomize, [FromQuery, Required] string sortAfter = null, [FromQuery, Required] bool asc = true, [FromQuery] int playFromIndex = 0)
        {
            var favoriteSongCount = await this.playlistService.GetFavoriteSongCount();
            var favoriteSongs = await this.playlistService.GetFavorites(playFromIndex, favoriteSongCount, sortAfter, asc, null);
            return Ok(await this.queueService.CreateQueue(favoriteSongs.Songs, randomize));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.Default)]
        public async Task<IActionResult> GetCurrentQueue()
        {
            return Ok(await this.queueService.GetCurrentQueue());
        }

        [HttpDelete]
        [Route(ApiRoutes.Queue.Default)]
        public async Task<IActionResult> ClearCurrentQueue()
        {
            await this.queueService.ClearQueue();
            return Ok();
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CurrentSong)]
        public async Task<IActionResult> GetCurrentSong()
        {
            return Ok(await this.queueService.GetCurrentSongInQueue());
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SkipForward)]
        public async Task<IActionResult> SkipForwardInQueue([FromQuery]int index = 0)
        {
            if (index < 1)
            {
                return Ok(await this.queueService.SkipForwardInQueue());
            }

            return Ok(await this.queueService.SkipForwardInQueue(index));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SkipBack)]
        public async Task<IActionResult> SkipBackInQueue()
        {
            return Ok(await this.queueService.SkipBackInQueue());
        }

        [HttpPost]
        [Route(ApiRoutes.Queue.RemoveSongsFromQueue)]
        public async Task<IActionResult> RemoveSongsInQueue([FromBody, Required] SongsToRemove request)
        {
            return Ok(await this.queueService.RemoveSongsWithIndexFromQueue(request.OrderIds));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.PushSongInQueue)]
        public async Task<IActionResult> PushSongsInQueue([FromQuery, Required] int srcIndex, [FromQuery, Required] int targetIndex)
        {
            return Ok(await this.queueService.PushSongToIndex(srcIndex, targetIndex));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.RandomizeQueue)]
        public async Task<IActionResult> RandomizeQueue([FromQuery]Guid playlistId, [FromQuery] Guid albumId, [FromQuery] Guid songId)
        {
            if (playlistId == Guid.Empty && albumId == Guid.Empty && songId == Guid.Empty)
            {
                // Get favroites
                var favoriteSongCount = await this.playlistService.GetFavoriteSongCount();
                var favorites = await this.playlistService.GetFavorites(0, favoriteSongCount, null, true, null);
                return Ok(await this.queueService.RandomizeQueue(favorites.Songs));
            }

            if (playlistId != Guid.Empty)
            {
                // Get playlist
                var playlistSongCount = await this.playlistService.GetPlaylistSongCount(playlistId);
                var playlistSongs = await this.playlistService.GetSongsInPlaylist(playlistId, 0, playlistSongCount, null, true, null);
                return Ok(await this.queueService.RandomizeQueue(playlistSongs.Songs));
            }

            if (albumId != Guid.Empty)
            {
                // Get Album songs
                var albumSongCount = await this.songService.GetSongCountOfAlbum(albumId);
                var albumSongs = await this.songService.GetSongsInAlbum(albumId, 0, albumSongCount);
                return Ok(await this.queueService.RandomizeQueue(albumSongs.Songs));
            }

            // TOOD: Add method for only playing one song and the randomized songs are taken from the album

            return BadRequest();

        }


    }
}
