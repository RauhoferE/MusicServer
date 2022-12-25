using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicServer.Const;
using MusicServer.Core.Const;

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
        [Authorize(Roles = RolesEnum.Admin)]
        public async  Task<IActionResult> Test()
        {
            return Ok();
        }
    }
}
