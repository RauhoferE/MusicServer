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
    }
}
