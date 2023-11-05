using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MusicServer.Hubs
{
    //[Authorize]
    public class StreamingHub : Hub
    {
        // Possible params if the user wants to have a public session
        //private readonly ConcurrentDictionary<string, List<string>> SessionParameters;

        // TODO: Replace with database tables

        // Contains mappuing from connection id to email
        private readonly ConcurrentDictionary<string, string> UserIdentifier;

        private readonly ConcurrentDictionary<string, List<string>> Sessions;

        // Contains the songs
        private readonly ConcurrentDictionary<string, List<Guid>> SharedQueue;

        // Contains info about the currently playling song
        private readonly ConcurrentDictionary<string, SongStreamDto> CurrenlyPlaying;

        public StreamingHub()
        {
            this.Sessions = new ConcurrentDictionary<string, List<string>>();
            this.SharedQueue = new ConcurrentDictionary<string, List<Guid>>();
            this.CurrenlyPlaying = new ConcurrentDictionary<string, SongStreamDto>();
        }

        public async IAsyncEnumerable<byte> StreamSong(Guid songId, IFileService fileService)
        {
            await foreach (var item in fileService.GetSongStream(songId))
            {
                yield return item;
            }
        }

        public async Task SendMessage(string user, string message)
    => await Clients.All.SendAsync("ReceiveMessage", user, message);

        public async Task<bool> CreateSession()
        {
            // False if the session already exists
            var addSuccess = this.Sessions.TryAdd(this.Context.ConnectionId, new List<string>());

            if (!addSuccess)
            {
                return false;
            }

            // TODO: Uncomment later
            //this.UserIdentifier.TryAdd(this.Context.ConnectionId, this.Context.User.Identity.Name);
            return true;
        }

        public async Task<UserSessionDto[]> GetCurrentSession(MusicServerDBContext dBContext)
        {
            List<UserSessionDto> sessionDtos = new List<UserSessionDto>();
            var sessionIds = this.Sessions.Keys.ToArray();

            foreach (var sessionId in sessionIds)
            {
                if (this.UserIdentifier.TryGetValue(sessionId, out var userId))
                {
                    User userEntity = dBContext.Users.FirstOrDefault(x => x.Email == userId);
                    sessionDtos.Add(new UserSessionDto()
                    {
                        Id = userEntity.Id,
                        ConnectionId = sessionId,
                        UserName = userEntity.Email
                    });
                    continue;
                }

                sessionDtos.Add(new UserSessionDto()
                {
                    ConnectionId = sessionId,
                    UserName = sessionId,
                    Id = -1
                });
            }

            return sessionDtos.ToArray();
        }

        public async Task JoinSessionRequest(string leaderId)
        {
            if (this.UserIdentifier.TryGetValue(this.Context.ConnectionId, out string userEmail))
            {
                // User already has joined a session or controls a session
                throw new UserSessionException("You have to leave the current session to join a new one!");
            }

            // TODO: Uncomment
            //var userIdentity = this.Context.User.Identity.Name;
            var userIdentity = this.Context.ConnectionId;

            // Ask the leader of the session if user can join it
            await this.Clients.User(leaderId).SendCoreAsync("JoinRequest", new string[2]
            {
                this.Context.ConnectionId,
                userIdentity
            });
        }

        public async Task AllowJoin(string joinId, string joinEmail, SongStreamDto songStreamDto)
        {
            var oldListenerList = this.Sessions.GetValueOrDefault(this.Context.ConnectionId);

            if (oldListenerList == null)
            {
                return;
            }

            // User is already joined
            if (!string.IsNullOrEmpty(oldListenerList.FirstOrDefault(joinId)))
            {
                return;
            }

            // Add new user
            oldListenerList.Add(joinId);

            // Throw Exception if User is already in a session
            if (!this.UserIdentifier.TryAdd(joinId, joinEmail))
            {
                throw new UserSessionException("User is already part of a session");
            }

            // Add New User to Session
            this.Sessions.TryUpdate(this.Context.ConnectionId, oldListenerList, this.Sessions.GetValueOrDefault(this.Context.ConnectionId));

            // Update the current information of the song state
            this.CurrenlyPlaying.TryUpdate(this.Context.ConnectionId, songStreamDto, this.CurrenlyPlaying.GetValueOrDefault(this.Context.ConnectionId));


            List< UserSessionDto > userSessionDtos = new List< UserSessionDto >();
            foreach (var connectionId in oldListenerList)
            {
                // DOnt add the current user to the user list
                if (this.Context.ConnectionId == connectionId)
                {
                    continue;
                }

                userSessionDtos.Add(new UserSessionDto()
                {
                    ConnectionId = connectionId,
                    Id = -1,
                    UserName = connectionId,
                });
            }
            // Send a success message with the leader id, the list of other users in the session and the current information of the song
            await this.Clients.User(joinId).SendAsync("JoinSuccess", this.Context.ConnectionId, userSessionDtos, songStreamDto);
        }

        public async Task ChangeSongState(string leaderId, SongStreamDto songStreamDto)
        {
            if (!this.Sessions.TryGetValue(leaderId, out List<string> listeners))
            {
                throw new UserSessionException("Session not found");
            }

            if (leaderId != this.Context.ConnectionId && !listeners.Contains(this.Context.ConnectionId))
            {
                throw new UnauthorizedAccessException("Not allowed to access the session!");
            }

            // Already song playing
            if (this.CurrenlyPlaying.TryGetValue(leaderId, out SongStreamDto oldSongStreamDto))
            {
                this.CurrenlyPlaying.TryUpdate(leaderId, songStreamDto, oldSongStreamDto);
            }
            else
            {
                // Add new if the session was just created
                this.CurrenlyPlaying.TryAdd(leaderId, songStreamDto);
            }

            if (leaderId == this.Context.ConnectionId)
            {
                // Send new State to listeners
                await this.Clients.Users(listeners).SendAsync("ChangeSongState", songStreamDto);
                return;
            }

            // Send state to everyone except the one that made the change
            await this.Clients.Users(
                listeners.Where(x => x != this.Context.ConnectionId).Concat(new string[1] {leaderId}))
                .SendAsync("ChangeSongState", songStreamDto);
        }

        public async Task CloseSession()
        {

        }

        public async Task LeaveSession(string leaderId)
        {
            if (!this.Sessions.TryGetValue(leaderId, out List<string> listeners))
            {
                throw new UserSessionException("Session not found");
            }

            // IF the Leader tries to leave kill the session
            if (this.Context.ConnectionId == leaderId)
            {
                this.CurrenlyPlaying.TryGetValue(leaderId, out SongStreamDto songStreamDto);
                this.UserIdentifier.TryGetValue(leaderId, out string userEmail);

                foreach (var listener in listeners)
                {

                }
            }
        }

        // Remove User from any session or kill the session if the user was the leader
        public override async Task OnDisconnectedAsync(Exception? exception)
        {

        }


    }
}
