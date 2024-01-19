using AutoMapper;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MusicServer.Const;
using MusicServer.Entities.Requests.Multi;
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
            await this.userService.SubscribeToUserAsync(userId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.SubscribeUser)]
        public async Task<IActionResult> UnSubscribeFromUser([FromRoute, Required] long userId)
        {
            await this.userService.UnsubscribeFromUserAsync(userId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.SubscribeArtist)]
        public async Task<IActionResult> SubscribeToArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.SuscribeToArtistAsync(artistId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.SubscribeArtist)]
        public async Task<IActionResult> UnSubscribeFromArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.UnsubscribeFromArtistAsync(artistId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.ReceiveNotificationsArtist)]
        public async Task<IActionResult> ReceiveNotificationsFromArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.ActivateNotificationsFromArtistAsync(artistId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.ReceiveNotificationsArtist)]
        public async Task<IActionResult> RemoveNotificationsFromArtist([FromRoute, Required] Guid artistId)
        {
            await this.userService.DeactivateNotificationsFromArtistAsync(artistId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.ReceiveNotificationsUser)]
        public async Task<IActionResult> ReceiveNotificationsFromUser([FromRoute, Required] long userId)
        {
            await this.userService.ActivateNotificationsFromUserAsync(userId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.ReceiveNotificationsUser)]
        public async Task<IActionResult> RemoveNotificationsFromUser([FromRoute, Required] long userId)
        {
            await this.userService.DeactivateNotificationsFromUserAsync(userId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetFollowedArtists)]
        public async Task<IActionResult> GetSubscribedArtists([FromRoute, Required] long userId, [FromQuery, Required] string query)
        {
            return Ok(await this.userService.GetFollowedArtistsAsync(userId, query));
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetFollowedUsers)]
        public async Task<IActionResult> GetSubscribedUsers([FromRoute, Required] long userId, [FromQuery, Required] string query)
        {
            return Ok(await this.userService.GetFollowedUsersAsync(userId, query));
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetUsers)]
        public async Task<IActionResult> GetUserList([FromQuery, Required] QueryPaginationSearchRequest request)
        {
            return Ok(await this.userService.GetUsersAsyncAsync(request.Page, request.Take, request.Query, request.Asc));
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetUser)]
        public async Task<IActionResult> GetUser([FromRoute, Required] long userId)
        {
            return Ok(await this.userService.GetUserAsync(userId));
        }

        [HttpPost]
        [Route(ApiRoutes.User.GetUser)]
        public async Task<IActionResult> EditUser([FromRoute, Required] long userId, [FromBody, Required] EditUser request)
        {
            await this.userService.ModifyUserAsync(userId, request);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.RoleUser)]
        public async Task<IActionResult> AddRoleToUser([FromRoute, Required] long userId, [FromRoute, Required] long roleId)
        {
            await this.userService.AddRoleToUserAsync(userId, roleId);
            return NoContent();
        }

        [HttpDelete]
        [Route(ApiRoutes.User.RoleUser)]
        public async Task<IActionResult> RemoveRoleFromUser([FromRoute, Required] long userId, [FromRoute, Required] long roleId)
        {
            await this.userService.RemoveRoleFromUserAsync(userId, roleId);
            return NoContent();
        }

        [HttpGet]
        [Route(ApiRoutes.User.Roles)]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await this.userService.GetRolesAsync());
        }

        [HttpGet]
        [Route(ApiRoutes.User.GetFollowedEntiies)]
        public async Task<IActionResult> GetFollowedEntities([FromQuery] string filter = "", [FromQuery] string searchTerm = "")
        {
            return Ok(await this.userService.GetAllFollowedUsersArtistsPlaylistsFavoritesAsync(filter, searchTerm));
        }
    }
}
