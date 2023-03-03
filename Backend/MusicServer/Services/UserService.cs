using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
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

        public async Task<GuidNameDto[]> GetFollowedArtists(int page, int take)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .ThenInclude(x => x.Artist)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            return this.mapper.Map<GuidNameDto[]>(targetUser.FollowedArtists.Skip((page - 1) * take).Take(take).ToArray());
        }

        public async Task<LongNameDto[]> GetFollowedUsers(int page, int take)
        {
            var targetUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .ThenInclude(x => x.FollowedUser)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            return this.mapper.Map<LongNameDto[]>(targetUser.FollowedUsers.Skip((page - 1) * take).Take(take).ToArray());
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
    }
}
