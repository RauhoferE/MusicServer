using AutoMapper;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicServer.Const;
using MusicServer.Entities.Requests.User;
using MusicServer.Interfaces;
using MusicServer.Settings;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MusicServer.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Base)]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public UserController(
            IUserService userService,
            IMapper mapper)
        {
            this.mapper = mapper;
            this.userService = userService;
        }

        [HttpGet]
        [Route(ApiRoutes.User.SubscribeUser)]
        public async Task<IActionResult> SubscribeToUser([FromRoute, Required] long userId)
        {
            await this.userService.SubscribeToUser(userId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.SubscribeUser)]
        public async Task<IActionResult> UnSubscribeFromUser([FromRoute, Required] long userId)
        {
            await this.userService.UnsubscribeFromUser(userId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.SubscribeArtist)]
        public async Task<IActionResult> SubscribeToArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.SuscribeToArtist(artistId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.SubscribeArtist)]
        public async Task<IActionResult> UnSubscribeFromArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.UnsubscribeFromArtist(artistId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetFollowedArtists)]
        public async Task<IActionResult> GetSubscribedArtists([FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            return Ok(await this.userService.GetFollowedArtists(page, take));
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetFollowedUsers)]
        public async Task<IActionResult> GetSubscribedUsers([FromQuery, Required] int page, [FromQuery, Required] int take)
        {
            return Ok(await this.userService.GetFollowedUsers(page, take));
        }
    }
}
