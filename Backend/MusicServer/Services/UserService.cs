using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.User;
using MusicServer.Entities.Responses;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Serilog;
using System.Security.Claims;

namespace MusicServer.Services
{
    public class UserService : IUserService
    {
        private readonly MusicServerDBContext dBContext;
        private readonly IActiveUserService activeUserService;
        private readonly IMapper mapper;

        public UserService(MusicServerDBContext dbContext,
            IActiveUserService activeUserService,
            IMapper mapper)
        {
            this.dBContext= dbContext;
            this.activeUserService = activeUserService;
            this.mapper = mapper;
        }

        public async Task<GuidNamePaginationResponse> GetFollowedArtists(int page, int take)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            return new GuidNamePaginationResponse
            {
                Items = this.mapper.Map<GuidNameDto[]>(targetUser.FollowedArtists.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = targetUser.FollowedArtists.Count
            };
        }

        public async Task<LongNamePaginationResponse> GetFollowedUsers(int page, int take)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .ThenInclude(x => x.FollowedUser)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            return new LongNamePaginationResponse
            {
                Items = this.mapper.Map<LongNameDto[]>(targetUser.FollowedUsers.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = targetUser.FollowedUsers.Count
            };
        }

        public async Task<UserDetailsDto> GetUserAsync(long userId)
        {
            var user = this.dBContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Include(x => x.FollowedUsers)
                .Include(x => x.Playlists)
                .ThenInclude(x => x.Playlist)
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            return this.mapper.Map<UserDetailsDto>(user);
        }

        public async Task<FullUserPaginationResponse> GetUsersAsync(int page, int take, string searchTerm)
        {
            var users = this.dBContext.Users.Where(x => true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = this.dBContext.Users.Where(x => x.Email.Contains(searchTerm) || x.UserName.Contains(searchTerm));
            }

            return new FullUserPaginationResponse
            {
                Users = this.mapper.Map<FullUserDto[]>(users.Skip((page - 1) * take).Take(take).ToArray()),
                TotalCount = users.Count()
            };  
        }

        public async Task ModifyUserAsync(long userId, EditUser userRequest)
        {
            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            user.LockoutEnd = userRequest.LockoutEnd;
            user.DemandPasswordChange = userRequest.DemandPasswordChange;
            user.IsDeleted = userRequest.IsDeleted;
            user.EmailConfirmed = userRequest.EmailConfirmed;
            await this.dBContext.SaveChangesAsync();
        }

        public async Task SubscribeToUser(long userId)
        {
            var targetUser = this.dBContext.Users
                .FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .ThenInclude(x => x.FollowedUser)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            if (sourceUser.Id == userId)
            {
                throw new NotAllowedException();
            }

            if (sourceUser.FollowedUsers.FirstOrDefault(x => x.FollowedUser.Id == userId) != null)
            {
                throw new AlreadyAssignedException();
            }

            sourceUser.FollowedUsers.Add(new UserUser()
            {
                FollowedUser = targetUser
            });
            await this.dBContext.SaveChangesAsync();
        }

        public async Task SuscribeToArtist(Guid artistId)
        {
            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var artist = this.dBContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            if (sourceUser.FollowedArtists.FirstOrDefault(x => x.Artist.Id == artistId) != null)
            {
                throw new AlreadyAssignedException();
            }

            sourceUser.FollowedArtists.Add(new UserArtist()
            {
                Artist = artist
            });
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UnsubscribeFromArtist(Guid artistId)
        {
            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedArtist = sourceUser.FollowedArtists.FirstOrDefault(x => x.Artist.Id == artistId) ?? throw new ArtistNotFoundException();

            sourceUser.FollowedArtists.Remove(followedArtist);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UnsubscribeFromUser(long userId)
        {
            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .ThenInclude(x => x.FollowedUser)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedUser = sourceUser.FollowedUsers.FirstOrDefault(x => x.FollowedUser.Id == userId) ?? throw new UserNotFoundException();

            sourceUser.FollowedUsers.Remove(followedUser);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddRoleToUserAsync(long userId, long roleId)
        {
            var user = this.dBContext.Users
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            var role = this.dBContext.Roles.FirstOrDefault(x => x.Id == roleId) ?? throw new MusicserverServiceException("Role not found");

            if (role.Id == (long)Roles.Root)
            {
                throw new MusicserverServiceException("Cannot add Root Role to User");
            }

            if (user.UserRoles.FirstOrDefault(x => x.Role.Id == role.Id) != null)
            {
                throw new MusicserverServiceException("Role already assigned to User");
            }

            user.UserRoles.Add(new UserRole()
            {
                Role = role,
                RoleId = roleId
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUserAsync(long userId, long roleId)
        {
            var user = this.dBContext.Users
    .Include(x => x.UserRoles)
    .ThenInclude(x => x.Role)
    .FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            var role = this.dBContext.Roles.FirstOrDefault(x => x.Id == roleId) ?? throw new MusicserverServiceException("Role not found");
            var userRole = user.UserRoles.FirstOrDefault(x => x.Role.Id == roleId);

            if (userRole == null)
            {
                throw new MusicserverServiceException("Role is not assigned to User");
            }

            user.UserRoles.Remove(userRole);

            await this.dBContext.SaveChangesAsync();
        }

        public async Task<LongNameDto[]> GetRoles()
        {
            List<LongNameDto> roles = new List<LongNameDto>();
            int i = 1;
            foreach (string name in Enum.GetNames(typeof(Roles)))
            {
                roles.Add(new LongNameDto()
                {
                    Id = i,
                    Name = name
                });

                i = i + 1;
            }

            return roles.ToArray();
        }
    }
}
