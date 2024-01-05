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
        //TODO: Send loop mode
        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueAlbum)]
        public async Task<IActionResult> CreateQueueFromAlbum(Guid albumId, [FromQuery, Required] bool randomize, [FromQuery]int playFromIndex = 0)
        {
            var albumSongCount = await this.songService.GetSongCountOfAlbum(albumId);
            var albumSongs = await this.songService.GetSongsInAlbum(albumId, 0, albumSongCount);
            await this.queueService.UpdateQueueData(albumId, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Album, randomize, true);
            return Ok(await this.queueService.CreateQueue(albumSongs.Songs, randomize, playFromIndex));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueuePlaylist)]
        public async Task<IActionResult> CreateQueueFromPlaylist(Guid playlistId, [FromQuery, Required] bool randomize, [FromQuery] string sortAfter = null, [FromQuery] bool asc = true, [FromQuery] int playFromOrder = 0)
        {
            var playlistSongCount = await this.playlistService.GetPlaylistSongCount(playlistId);
            var playlistSongs = await this.playlistService.GetSongsInPlaylist(playlistId, 0, playlistSongCount, sortAfter, asc, null);
            await this.queueService.UpdateQueueData(playlistId, LoopMode.None, sortAfter, QueueTarget.Playlist, randomize, asc);
            return Ok(await this.queueService.CreateQueue(playlistSongs.Songs, randomize, playFromOrder));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueFavorites)]
        public async Task<IActionResult> CreateQueueFromFavorites([FromQuery, Required] bool randomize, [FromQuery] string sortAfter = null, [FromQuery] bool asc = true, [FromQuery] int playFromOrder = 0)
        {
            var favoriteSongCount = await this.playlistService.GetFavoriteSongCount();
            var favoriteSongs = await this.playlistService.GetFavorites(0, favoriteSongCount, sortAfter, asc, null);
            await this.queueService.UpdateQueueData(Guid.Empty, LoopMode.None, sortAfter, QueueTarget.Favorites, randomize, asc);
            return Ok(await this.queueService.CreateQueue(favoriteSongs.Songs, randomize, playFromOrder));
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.CreateQueueSingleSong)]
        public async Task<IActionResult> CreateQueueFromSingleSong(Guid songId, [FromQuery, Required] bool randomize)
        {
            var song = await this.songService.GetSongInformation(songId);
            await this.queueService.UpdateQueueData(songId, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Song, randomize, true);

            if (randomize)
            {
                var albumCount = await this.songService.GetSongCountOfAlbum(song.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbum(song.Album.Id, 0, albumCount);
                albumSongs.Songs = albumSongs.Songs.Where(x => x.Id != songId).Prepend(song).ToArray();
                return Ok(await this.queueService.CreateQueue(albumSongs.Songs, randomize, 0));
            }

            return Ok(await this.queueService.CreateQueue(new[] {song} , false, -1));

        }

        [HttpGet]
        [Route(ApiRoutes.Queue.Default)]
        public async Task<IActionResult> GetCurrentQueue()
        {
            return Ok(await this.queueService.GetCurrentQueue());
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.SongWithIndex)]
        public async Task<IActionResult> GetSongWithIndexFromQueue([FromRoute, Required] int index)
        {
            return Ok(await this.queueService.GetSongInQueueWithIndex(index));
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

        [HttpPost]
        [Route(ApiRoutes.Queue.AddSongsToQueue)]
        public async Task<IActionResult> AddSongsInQueue([FromBody, Required] SongsToPlaylist request)
        {
            await this.queueService.AddSongsToQueue(request.SongIds);
            return Ok();
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
                await this.queueService.UpdateQueueData(Guid.Empty, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Favorites, true, true);
                return Ok(await this.queueService.RandomizeQueue(favorites.Songs));
            }

            if (playlistId != Guid.Empty)
            {
                // Get playlist
                var playlistSongCount = await this.playlistService.GetPlaylistSongCount(playlistId);
                var playlistSongs = await this.playlistService.GetSongsInPlaylist(playlistId, 0, playlistSongCount, null, true, null);
                await this.queueService.UpdateQueueData(playlistId, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Playlist, true, true);
                return Ok(await this.queueService.RandomizeQueue(playlistSongs.Songs));
            }

            if (albumId != Guid.Empty)
            {
                // Get Album songs
                var albumSongCount = await this.songService.GetSongCountOfAlbum(albumId);
                var albumSongs = await this.songService.GetSongsInAlbum(albumId, 0, albumSongCount);
                await this.queueService.UpdateQueueData(albumId, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Album, true, true);
                return Ok(await this.queueService.RandomizeQueue(albumSongs.Songs));
            }

            if (songId != Guid.Empty)
            {
                var songDetails = await this.songService.GetSongInformation(songId);
                var albumSongCount = await this.songService.GetSongCountOfAlbum(songDetails.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbum(songDetails.Album.Id, 0, albumSongCount);
                await this.queueService.UpdateQueueData(songId, LoopMode.None, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Song, true, true);
                return Ok(await this.queueService.RandomizeQueue(albumSongs.Songs));
            }

            return BadRequest();
        }

        [HttpGet]
        [Route(ApiRoutes.Queue.QueueData)]
        public async Task<IActionResult> GetQueueData()
        {
            return Ok(await this.queueService.GetQueueData());
        }

        [HttpPost]
        [Route(ApiRoutes.Queue.QueueData)]
        public async Task<IActionResult> UpdateQueueData([FromQuery, Required]Guid itemId, [FromQuery, Required] string loopMode, [FromQuery, Required] string sortAfter, [FromQuery, Required] string target, [FromQuery, Required] bool randomize, [FromQuery, Required] bool asc)
        {
            await this.queueService.UpdateQueueData(itemId, loopMode, sortAfter, target, randomize, asc);
            return Ok(await this.queueService.GetQueueData());
        }


    }
}
