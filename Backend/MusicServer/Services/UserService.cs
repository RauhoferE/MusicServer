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
using MusicServer.Helpers;
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

        public async Task<GuidNamePaginationResponse> GetFollowedArtistsAsync(int page, int take, string query, bool asc)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedArtists = SortingHelpers.SortSearchFollowedArtists(targetUser.FollowedArtists.AsQueryable(), asc, query);

            var mappedArtists = this.mapper.Map<GuidNameDto[]>(targetUser.FollowedArtists.Skip((page - 1) * take).Take(take).ToArray());

            foreach (var artist in mappedArtists)
            {
                artist.FollowedByUser = true;
            }

            return new GuidNamePaginationResponse
            {
                Items = mappedArtists,
                TotalCount = followedArtists.Count()
            };
        }

        public async Task<UserDtoPaginationResponse> GetFollowedUsersAsync(int page, int take, string query, bool asc)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .ThenInclude(x => x.FollowedUser)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedUsers = SortingHelpers.SortSearchFollowedUsers(targetUser.FollowedUsers.AsQueryable(), asc, query);

            var mappedUsers = this.mapper.Map<UserDto[]>(targetUser.FollowedUsers.Skip((page - 1) * take).Take(take).ToArray());

            return new UserDtoPaginationResponse
            {
                Users = mappedUsers,
                TotalCount = followedUsers.Count()
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

        public async Task<FullUserPaginationResponse> GetUsersAsyncAsync(int page, int take, string searchTerm, bool asc)
        {
            var users = this.dBContext.Users.Where(x => true);

            users = SortingHelpers.SortSearchUsers(users, asc, searchTerm);

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

        public async Task SubscribeToUserAsync(long userId)
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

        public async Task SuscribeToArtistAsync(Guid artistId)
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

        public async Task UnsubscribeFromArtistAsync(Guid artistId)
        {
            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedArtist = sourceUser.FollowedArtists.FirstOrDefault(x => x.Artist.Id == artistId) ?? throw new ArtistNotFoundException();

            sourceUser.FollowedArtists.Remove(followedArtist);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UnsubscribeFromUserAsync(long userId)
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

        public async Task<LongNameDto[]> GetRolesAsync()
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

        
        public async Task<AllFollowedEntitiesResponse> GetAllFollowedUsersArtistsPlaylistsFavoritesAsync(string filter, string searchTerm)
        {
            filter = filter ?? string.Empty;
            searchTerm = searchTerm ?? string.Empty;
            searchTerm = searchTerm.ToLower();
            filter = filter.ToLower();
            

            // TODO: Maybe useless with IActiveUser
            var user = this.dBContext.Users
                    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var favoritesCount = this.dBContext.Users
                .Include(x => x.Favorites)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id).Favorites.Count();

            IEnumerable<UserArtist> followedArtists = new List<UserArtist>();

            IEnumerable<Playlist> followedPlaylists = new List<Playlist>();

            IEnumerable<UserUser> followedUsers = new List<UserUser>();

            if (filter.ToLower() == "artists" || filter == string.Empty )
            {
                followedArtists = this.dBContext.Users
    .Include(x => x.FollowedArtists)
    .ThenInclude(x => x.Artist)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id).FollowedArtists
    .Where(x => x.Artist.Name.ToLower().Contains(searchTerm))
    .OrderBy(x => x.Id)
    .Take(100)
    .ToList();
            }

            if (filter.ToLower() == "playlists" || filter == string.Empty)
            {
                followedPlaylists = this.dBContext.Users
    .Include(x => x.Playlists)
    .ThenInclude(x => x.Playlist)
    .ThenInclude(x => x.Songs)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id).Playlists
    .Where(x => x.Playlist.Name.ToLower().Contains(searchTerm))
    .OrderBy(x => x.Id)
    .Take(100)
    .ToList().Select(x => x.Playlist);
            }

            if (filter.ToLower() == "users" || filter == string.Empty)
            {
                followedUsers = this.dBContext.Users
    .Include(x => x.FollowedUsers)
    .ThenInclude(x => x.FollowedUser)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id).FollowedUsers
    .Where(x => x.FollowedUser.UserName.ToLower().Contains(searchTerm))
    .OrderBy(x => x.Id)
    .Take(100)
    .ToList();
            }

            var mappedFollowedPlaylists = this.mapper.Map<FollowedPlaylistDto[]>(followedPlaylists);

            foreach (var pl in mappedFollowedPlaylists)
            {
                var playlistEntity = this.dBContext
                    .PlaylistUsers
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.IsCreator == true && x.Playlist.Id == pl.Id)
                    ?? throw new PlaylistNotFoundException("Can't get Creator of Playlist.");

                pl.CreatorName = playlistEntity.User.UserName;
            }
            
            return new AllFollowedEntitiesResponse()
            {
                FavoritesSongCount = favoritesCount,
                FollowedUsers = this.mapper.Map<UserDto[]>(followedUsers),
                FollowedArtists = this.mapper.Map<GuidNameDto[]>(followedArtists),
                FollowedPlaylists = mappedFollowedPlaylists
            };
        }

        public async Task ActivateNotificationsFromArtistAsync(Guid artistId)
        {
            var followedArtist = this.dBContext.FollowedArtists
    .Include(x => x.Artist)
    .Include(x => x.User)
    .FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.Artist.Id == artistId);

            // IF player already follows artist
            if (followedArtist != null)
            {
                followedArtist.ReceiveNotifications = true;
                await this.dBContext.SaveChangesAsync();
                return;
            }

            var artist = this.dBContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            var user = this.dBContext.Users.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            this.dBContext.FollowedArtists.Add(new UserArtist()
            {
                Artist = artist,
                User = user,
                ReceiveNotifications = true
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task DeactivateNotificationsFromArtistAsync(Guid artistId)
        {
            var followedArtist = this.dBContext.FollowedArtists
.Include(x => x.Artist)
.Include(x => x.User)
.FirstOrDefault(x => x.User.Id == this.activeUserService.Id && x.Artist.Id == artistId) ?? throw new DataNotFoundException();

            followedArtist.ReceiveNotifications = false;
            await this.dBContext.SaveChangesAsync();
        }
    }
}
