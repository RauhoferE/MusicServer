﻿using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public UserService(MusicServerDBContext dbContext,
            IActiveUserService activeUserService)
        {
            this.dBContext= dbContext;
            this.activeUserService = activeUserService;
        }

        public async Task SubscribeToUser(Guid userId)
        {
            var targetUser = this.dBContext.Users
                .FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedUsers)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            if (sourceUser.FollowedUsers.FirstOrDefault(x => x.Id == userId) != null)
            {
                throw new AlreadyAssignedException();
            }

            sourceUser.FollowedUsers.Add(targetUser);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task SuscribeToArtist(Guid artistId)
        {
            var sourceUser = this.dBContext.Users
                .Include(x => x.FollowedArtists)
                .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var artist = this.dBContext.Artists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            if (sourceUser.FollowedArtists.FirstOrDefault(x => x.Id == artistId) != null)
            {
                throw new AlreadyAssignedException();
            }

            sourceUser.FollowedArtists.Add(artist);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UnsubscribeFromArtist(Guid artistId)
        {
            var sourceUser = this.dBContext.Users
    .Include(x => x.FollowedArtists)
    .FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedArtist = sourceUser.FollowedArtists.FirstOrDefault(x => x.Id == artistId) ?? throw new ArtistNotFoundException();

            sourceUser.FollowedArtists.Remove(followedArtist);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task UnsubscribeFromUser(Guid userId)
        {
            var sourceUser = this.dBContext.Users
.Include(x => x.FollowedUsers)
.FirstOrDefault(x => x.Id == this.activeUserService.Id) ?? throw new UserNotFoundException();

            var followedUser = sourceUser.FollowedUsers.FirstOrDefault(x => x.Id == userId) ?? throw new UserNotFoundException();

            sourceUser.FollowedUsers.Remove(followedUser);
            await this.dBContext.SaveChangesAsync();
        }
    }
}
