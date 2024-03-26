using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.HubEntities;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using Serilog;
using System;

namespace MusicServer.Hubs
{
    [Authorize]
    public class StreamingHub : Hub<IStreamingHub>
    {
        private readonly IStreamingService streamingService;

        private readonly IActiveUserService activeUserService;

        private readonly IQueueService queueService;

        public StreamingHub(IStreamingService streamingService, IActiveUserService activeUserService, IQueueService queueService)
        {
            this.streamingService = streamingService;
            this.activeUserService = activeUserService;
            this.queueService = queueService;
        }

        // Maybe replace the string of groupid with guid
        public async Task JoinSession(string groupId)
        {
            // Put this check in every method
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            // Remove user from old group and delete it
            if (await this.streamingService.IsUserAlreadyInGroupAsync(this.activeUserService.Id.ToString(), false))
            {
                var gName = await this.streamingService.GetGroupName(this.Context.ConnectionId);
                await this.streamingService.DeleteGroupAsync(Guid.Parse(gName));
                await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, gName);
            }

            if (!(await this.streamingService.JoinGroup(g, this.activeUserService.Id.ToString(), this.Context.ConnectionId)))
            {
                throw new HubException("Error when joining group");
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            var masterId = await this.streamingService.GetConnectionIdOfMaster(g);
            await this.Clients.GroupExcept(groupId, this.Context.ConnectionId).UserJoinedSession(this.activeUserService.Email);
        }

        public async Task SendCurrentSongToJoinedUser(string groupId, string joinedUserConnectionId, CurrentPlayerData playerData)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }
            // Get current song and media info and send it to all
            // This includes
            // Current Song, is player on random, current seconds, loopmode, isPlaying
            // Song will only be played on host
            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            if ((await this.streamingService.GetConnectionIdOfMaster(g)) != this.Context.ConnectionId)
            {
                throw new HubException("Error user is not master.");
            }

            var currentSong = await this.queueService.GetCurrentSongInQueueAsync();

            await this.Clients.Client(joinedUserConnectionId).GetPlayerData(playerData);
            await this.Clients.Client(joinedUserConnectionId).GetCurrentPlayingSong(currentSong);
        }

        public async Task GetCurrentQueue(string groupId)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Caller.GetQueue(queue);
        }

        public async Task AddSongsToQueue(string groupId, Guid[] songIds)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            await this.queueService.AddSongsToQueueOfUserAsync(masterId, songIds);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task SkipBackInQueue(string groupId)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            var currentSong = await this.queueService.SkipBackInQueueOfUserAsync(masterId);
            //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
            await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
        }

        public async Task SkipForwardInQueue(string groupId, int index = 0)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);

            PlaylistSongDto currentSong = null;

            if (index < 1)
            {
                currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId);
            }


            if (index > 0)
            {
                currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId, index);
            }

            //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);

            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
            await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
        }

        //public async Task SkipForwardInQueue(string groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    var currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId);
        //    // This is done in case a user is in the queue view
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId).GetQueue(queue);
        //}

        public async Task ClearQueue(string groupId)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            await this.queueService.ClearManuallyAddedQueueOfUserAsync(masterId);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task RemoveSongsInQueue(string groupId, int[] orderIds)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            await this.queueService.RemoveSongsWithIndexFromQueueOfUserAsync(masterId, orderIds);
            // This is done in case a user is in the queue view
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task PushSongInQueue(string groupId, int srcIndex, int targetIndex, int markAsAddedManually = -1)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(g);
            await this.queueService.PushSongToIndexOfUserAsync(masterId, srcIndex, targetIndex, markAsAddedManually);
            // This is done in case a user is in the queue view
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task UpdatePlayerData(string groupId, CurrentPlayerData playerData)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.Clients.GroupExcept(groupId, this.Context.ConnectionId).GetPlayerData(playerData);
        }

        public async Task LeaveGroup(string groupId)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.GroupExistsAsync(g)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            // Remove User from group
            var resp = await this.streamingService.DeleteUserWithConnectionId(this.Context.ConnectionId);
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId);

            // Send info that user disconnected
            await this.Clients.Group(groupId).UserDisconnected(resp.Email);

            // Create a new group with only the user
            var newGuid = Guid.NewGuid();

            if (!(await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId)))
            {
                newGuid = Guid.NewGuid();
                await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId);
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());
            await this.Clients.Caller.GetGroupName(newGuid);

            if (!resp.IsMaster)
            {
                return;
            }

            // Remove group if user was master
            await this.RemoveGroup(groupId);


        }

        private async Task RemoveGroup(string groupId)
        {
            // If User is master remove all other users from group
            // And send notification that the group has been deleted
            var deleteGroupResponses = await this.streamingService.DeleteGroupAsync(Guid.Parse(groupId));

            await this.Clients.Group(groupId).GroupDeleted();
            foreach (var dgr in deleteGroupResponses)
            {
                await this.Groups.RemoveFromGroupAsync(dgr.ConnectionId, groupId);

                var newGuid = Guid.NewGuid();

                if (!(await this.streamingService.CreateGroupAsync(newGuid, dgr.ConnectionId, dgr.ConnectionId)))
                {
                    newGuid = Guid.NewGuid();
                    await this.streamingService.CreateGroupAsync(newGuid, dgr.ConnectionId, dgr.ConnectionId);
                }
                await this.Groups.AddToGroupAsync(dgr.ConnectionId, newGuid.ToString());
                await this.Clients.Client(dgr.ConnectionId).GetGroupName(newGuid);
            }
        }

        public override async Task OnConnectedAsync()
        {
            Log.Information($"User connected {this.Context.ConnectionId}");
            var newGuid = Guid.NewGuid();

            if (!(await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId)))
            {
                newGuid = Guid.NewGuid();
                await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId);
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());
            await this.Clients.Caller.GetGroupName(newGuid);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information($"User disconnected {this.Context.ConnectionId}");

            if (exception == null)
            {
                Log.Warning($"User disconnected unexpectatly {this.Context.ConnectionId}");
            }

            var groupId = await this.streamingService.GetGroupName(this.Context.ConnectionId);

            // IF the user didn't had a group just return
            if (string.IsNullOrEmpty(groupId))
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            // Remove User from group
            var resp = await this.streamingService.DeleteUserWithConnectionId(this.Context.ConnectionId);
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId);

            // Send info that user disconnected
            await this.Clients.Group(groupId).UserDisconnected(resp.Email);

            if (!resp.IsMaster)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            await this.RemoveGroup(groupId);


            await base.OnDisconnectedAsync(exception);
        }


    }
}
