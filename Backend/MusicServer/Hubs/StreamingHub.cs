using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.HubEntities;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;

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

        //public Task RemoveSongsInQueue(SongsToRemove request);

        // Creates a session and returns the sessionID aka the group name to the creator
        public async Task CreateSession()
        {
            var newGuid = Guid.NewGuid();

            if (!(await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId)))
            {
                newGuid = Guid.NewGuid();
                await this.streamingService.CreateGroupAsync(newGuid, this.activeUserService.Id.ToString(), this.Context.ConnectionId);
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());
            await this.Clients.Caller.GetSessionId(newGuid);
        }

        public async Task JoinSession(string groupId)
        {
            var g = Guid.Empty;
            if (!Guid.TryParse(groupId, out g))
            {
                throw new HubException("Error when parsing groupname");
            }

            if (!(await this.streamingService.JoinGroup(g, this.activeUserService.Id.ToString(), this.Context.ConnectionId)))
            {
                throw new HubException("Error when joining group");
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            var masterId = await this.streamingService.GetConnectionIdOfMaster(groupId);
            await this.Clients.GroupExcept(groupId, this.Context.ConnectionId).UserJoinedSession(this.activeUserService.Email);
        }

        public async Task SendCurrentSongToJoinedUser(string groupId, string joinedUserConnectionId, CurrentPlayerData playerData)
        {
            // Get current song and media info and send it to all
            // This includes
            // Current Song, is player on random, current seconds, loopmode, isPlaying
            // Song will only be played on host
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            if ((await this.streamingService.GetConnectionIdOfMaster(groupId)) != this.Context.ConnectionId)
            {
                throw new HubException("Error user is not master.");
            }

            await this.Clients.Client(joinedUserConnectionId).UpdatePlayerData(playerData);
        }

        public async Task GetCurrentQueue(string groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Caller.GetQueue(queue);
        }

        public async Task AddSongsToQueue(string groupId, SongsToPlaylist request)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            await this.queueService.AddSongsToQueueOfUserAsync(request.SongIds, masterId);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task SkipBackInQueue(string groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            var currentSong = await this.queueService.SkipBackInQueueOfUserAsync(masterId);
            //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task SkipForwardInQueue(string groupId, int index = 0)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            var currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId, index);
            //await this.Clients.Group(groupId).GetCurrentPlayingSong(currentSong);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task SkipForwardInQueue(string groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            var currentSong = await this.queueService.SkipForwardInQueueOfUserAsync(masterId);
            // This is done in case a user is in the queue view
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }

        public async Task ClearQueue(string groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var masterId = await this.streamingService.GetIdOfMaster(groupId);
            await this.queueService.ClearQueueOfUserAsync(masterId);
            var queue = await this.queueService.GetQueueOfUserAsync(masterId);
            await this.Clients.Group(groupId).GetQueue(queue);
        }


    }
}
