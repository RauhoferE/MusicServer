using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class DevelopmentController : Controller
    {
        public DevelopmentController()
        {

        }

        [HttpGet]
        [Route(ApiRoutes.Development.Test)]
        public async  Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
