using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Interfaces;

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

        public IActionResult Index()
        {
            return View();
        }
    }
}
