using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.HubEntities;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using MusicServer.Services;
using Serilog;
using System;

namespace MusicServer.Hubs
{
    [Authorize]
    public class StreamingHub : Hub<IStreamingHub>
    {
        private readonly IStreamingService streamingService;

        private readonly IActiveUserService activeUserService;

        private readonly IGroupQueueService groupQueueService;

        private readonly IQueueService queueService;

        private readonly IPlaylistService playlistService;

        private readonly ISongService songService;

        // This hub is strictly for listening music together
        // The queue controller is when listing alone
        // When a player joins a new group entitiy is created 
        // The group data and group queue is taken from the queue data and queue entities that are beeing handled in queue service
        // When the master disconnects the current queueu data and queue is discarded


        public StreamingHub(IStreamingService streamingService, IActiveUserService activeUserService, 
            IGroupQueueService groupQueueService, IPlaylistService playlistService, ISongService songService, 
            IQueueService queueService)
        {
            this.streamingService = streamingService;
            this.activeUserService = activeUserService;
            this.groupQueueService = groupQueueService;
            this.playlistService = playlistService; 
            this.songService = songService;
            this.queueService = queueService;

        }

        //public async Task JoinSession(Guid groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Group doesnt exist!");
        //    }

        //    // Check if user is already part of a group
        //    if (await this.streamingService.IsUserAlreadyPartOfGroupWithOthers(this.Context.ConnectionId))
        //    {
        //        throw new HubException("Error user is already in a group!");
        //    }

        //    // Delete the group with only the user as client
        //    var gName = await this.streamingService.GetGroupName(this.Context.ConnectionId);
        //    await this.streamingService.DeleteGroupAsync(gName);
        //    await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, gName.ToString());

        //    // Join the new group
        //    if (!(await this.streamingService.JoinGroup(groupId, this.activeUserService.Id, this.Context.ConnectionId, this.activeUserService.Email)))
        //    {
        //        throw new HubException("Error when joining group");
        //    }

        //    await this.Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

        //    var userList = await this.streamingService.GetEmailList(groupId);

        //    await this.Clients.Client(this.Context.ConnectionId).GetGroupName(groupId);
        //    await this.Clients.Client(this.Context.ConnectionId).GetUserList(userList);
        //    await this.Clients.GroupExcept(groupId.ToString(), this.Context.ConnectionId).UserJoinedSession(this.activeUserService.Email);
        //}

        //public async Task SendCurrentSongToJoinedUser(Guid groupId, string email, CurrentPlayerData playerData)
        //{
        //    // Get current song and media info and send it to all
        //    // This includes
        //    // Current Song, is player on random, current seconds, loopmode, isPlaying
        //    // Song will only be played on host
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    if ((await this.streamingService.GetConnectionIdOfMaster(groupId)) != this.Context.ConnectionId)
        //    {
        //        throw new HubException("Error user is not master.");
        //    }

        //    var joinedUserConnectionId = await this.streamingService.GetConnectionIdOfUser(email, groupId);

        //    var currentSong = await this.queueService.GetCurrentSongInQueueAsync();

        //    await this.Clients.Client(joinedUserConnectionId).GetCurrentPlayingSong(currentSong);
        //    await this.Clients.Client(joinedUserConnectionId).GetPlayerData(playerData);

        //}

        //public async Task GetCurrentQueue(Guid groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Caller.GetQueue(queue);
        //}

        //public async Task AddSongsToQueue(Guid groupId, Guid[] songIds)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    await this.queueService.AddSongsToQueueOfUserAsync(masterId, songIds);
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //}

        //public async Task SkipBackInQueue(Guid groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    var currentSong = await this.queueService.SkipBackInQueueOfUserAsync(masterId);
        //    //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //    await this.Clients.Group(groupId.ToString()).GetCurrentPlayingSong(currentSong);
        //}

        //public async Task SkipForwardInQueue(Guid groupId, int index = 0)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);

        //    PlaylistSongDto currentSong = null;

        //    if (index < 1)
        //    {
        //        currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId);
        //    }


        //    if (index > 0)
        //    {
        //        currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId, index);
        //    }

        //    //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);

        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //    await this.Clients.Group(groupId.ToString()).GetCurrentPlayingSong(currentSong);
        //} 

        //public async Task ClearQueue(Guid groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    await this.queueService.ClearManuallyAddedQueueOfUserAsync(masterId);
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //}

        //public async Task RemoveSongsInQueue(Guid groupId, int[] orderIds)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    await this.queueService.RemoveSongsWithIndexFromQueueOfUserAsync(masterId, orderIds);
        //    // This is done in case a user is in the queue view
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //}

        //public async Task PushSongInQueue(Guid groupId, int srcIndex, int targetIndex, int markAsAddedManually = -1)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    var masterId = await this.streamingService.GetIdOfMaster(groupId);
        //    await this.queueService.PushSongToIndexOfUserAsync(masterId, srcIndex, targetIndex, markAsAddedManually);
        //    // This is done in case a user is in the queue view
        //    var queue = await this.queueService.GetQueueOfUserAsync(masterId);
        //    await this.Clients.Group(groupId.ToString()).GetQueue(queue);
        //}

        //public async Task UpdatePlayerData(Guid groupId, CurrentPlayerData playerData)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    await this.Clients.GroupExcept(groupId.ToString(), this.Context.ConnectionId).GetPlayerData(playerData);
        //}

        //public async Task LeaveGroup(Guid groupId)
        //{
        //    if (!(await this.streamingService.GroupExistsAsync(groupId)))
        //    {
        //        throw new HubException("Error group doesnt exist.");
        //    }

        //    // Remove User from group
        //    var resp = await this.streamingService.DeleteUserWithConnectionId(this.Context.ConnectionId);
        //    await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId.ToString());

        //    // Send info that user disconnected
        //    await this.Clients.Group(groupId.ToString()).UserDisconnected(resp.Email);

        //    // Create a new group with only the user
        //    var newGuid = Guid.NewGuid();

        //    if (!(await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId, this.activeUserService.Email)))
        //    {
        //        newGuid = Guid.NewGuid();
        //        await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId, this.activeUserService.Email);
        //    }

        //    await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());
        //    await this.Clients.Caller.GetGroupName(newGuid);

        //    if (!resp.IsMaster)
        //    {
        //        return;
        //    }

        //    // Remove group if user was master
        //    await this.RemoveGroup(groupId);
        //}

        //private async Task RemoveGroup(Guid groupId)
        //{
        //    // If User is master remove all other users from group
        //    // And send notification that the group has been deleted
        //    var deleteGroupResponses = await this.streamingService.DeleteGroupAsync(groupId);

        //    await this.Clients.Group(groupId.ToString()).GroupDeleted();
        //    foreach (var dgr in deleteGroupResponses)
        //    {
        //        await this.Groups.RemoveFromGroupAsync(dgr.ConnectionId, groupId.ToString());

        //        var newGuid = Guid.NewGuid();

        //        if (!(await this.streamingService.CreateGroupAsync(newGuid, dgr.ConnectionId, dgr.ConnectionId, dgr.Email)))
        //        {
        //            newGuid = Guid.NewGuid();
        //            await this.streamingService.CreateGroupAsync(newGuid, dgr.ConnectionId, dgr.ConnectionId, dgr.Email);
        //        }
        //        await this.Groups.AddToGroupAsync(dgr.ConnectionId, newGuid.ToString());
        //        await this.Clients.Client(dgr.ConnectionId).GetGroupName(newGuid);
        //    }
        //}

        public override async Task OnConnectedAsync()
        {
            Log.Information($"User connected {this.Context.ConnectionId}");
            var newGuid = Guid.NewGuid();

            // Check if there is a groupo whrere user with id is master

            // Yes is master
            // Create a new entry with this groupname and and new connection id
            // And return groupname and userlist -> id distinct && id != active.id
            // Add new connection id to group

            // No such thing exists
            // Create a new group where user is master
            // and return the groupname
            // Add new connection id to group

            if (!(await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id, this.Context.ConnectionId, this.activeUserService.Email)))
            {
                newGuid = Guid.NewGuid();
                await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id, this.Context.ConnectionId, this.activeUserService.Email);
            }

            // Add the user with the connection id to the newly created group
            await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());

            // Return the groupname to the user
            await this.Clients.Caller.GetGroupName(newGuid);

            // Get the queue and queue data from the queue service
            var queueData = await this.queueService.GetQueueDataEntityAsync();
            var queueEntities = await this.queueService.GetAllQueueEntitiesAsync();

            // set the queue data as the group queue data and return it to the caller
            if (queueData != null)
            {
                await this.groupQueueService.SetQueueDataAsync(queueData);
                var data = await this.groupQueueService.GetQueueDataAsync(newGuid);
                await this.Clients.Client(this.Context.ConnectionId).GetQueueData(data);
            }

            // set the queue entities as the group queue entities and return the entities
            // and the current song to the caller
            if (queueEntities.Length > 0)
            {
                await this.groupQueueService.SetQueueEntitiesAsync(queueEntities);
                var queueSongs = await this.groupQueueService.GetCurrentQueueAsync(newGuid);
                queueSongs = await this.groupQueueService.MarkQueueSongsAsFavorite(this.activeUserService.Id, queueSongs);
                var currentSong = await this.groupQueueService.GetCurrentSongInQueueAsync(newGuid);
                currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);
                await this.Clients.Client(this.Context.ConnectionId).GetQueueEntities(queueSongs);
                await this.Clients.Client(this.Context.ConnectionId).GetCurrentPlayingSong(currentSong);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information($"User disconnected {this.Context.ConnectionId}");

            // When a user disconnects remove him from all groups and all entities related to that user
            // If the user is master remove the group and signal the group that the group has been deleted and remove him from the group.
            // If the user was not a master just signal the group that the user left. And remove him from the group
            // The rest is done in the frontend

            //if (exception == null)
            //{
            //    Log.Warning($"User disconnected unexpectatly {this.Context.ConnectionId}");
            //}

            //var groupId = await this.streamingService.GetGroupName(this.Context.ConnectionId);

            //// IF the user didn't had a group just return
            //if (Guid.Empty == groupId)
            //{
            //    await base.OnDisconnectedAsync(exception);
            //    return;
            //}

            //// Remove User from group
            //var resp = await this.streamingService.DeleteUserWithConnectionId(this.Context.ConnectionId);
            //await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId.ToString());

            //// Send info that user disconnected
            //await this.Clients.Group(groupId.ToString()).UserDisconnected(resp.Email);

            //if (!resp.IsMaster)
            //{
            //    await base.OnDisconnectedAsync(exception);
            //    return;
            //}

            //await this.RemoveGroup(groupId);


            await base.OnDisconnectedAsync(exception);
        }


    }
}
