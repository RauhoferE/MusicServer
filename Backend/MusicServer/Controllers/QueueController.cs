using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Interfaces;

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

        public async Task<IActionResult> CreateQueueFromAlbum()
        {
            return Ok();
        }

        public async Task<IActionResult> CreateQueueFromPlaylist()
        {
            return Ok();
        }

        public async Task<IActionResult> CreateQueueFromFavorites()
        {
            return Ok();
        }

        public async Task<IActionResult> GetCurrentQueue()
        {
            return Ok();
        }

        public async Task<IActionResult> ClearCurrentQueue()
        {
            return Ok();
        }

        public async Task<IActionResult> GetCurrentSong()
        {
            return Ok();
        }

        public async Task<IActionResult> SkipForwardInQueue()
        {
            return Ok();
        }

        public async Task<IActionResult> SkipBackInQueue()
        {
            return Ok();
        }

        public async Task<IActionResult> RemoveSongsInQueue()
        {
            return Ok();
        }

        public async Task<IActionResult> PushSongsInQueue()
        {
            return Ok();
        }


    }
}
