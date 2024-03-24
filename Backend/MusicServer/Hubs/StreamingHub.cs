using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;

namespace MusicServer.Hubs
{
    public class StreamingHub : Hub<IStreamingHub>
    {
        private readonly IStreamingService streamingService;

        public StreamingHub(IStreamingService streamingService)
        {
            this.streamingService = streamingService;
        }

        // Returns to the user who created it a unique Id
        //public Task CreateSession();

        //// This is done via a unique session Id
        //// The host is asced for current song data
        //// Then the current queue and song data is sent ot the joined user
        //public Task JoinSession(Guid sessionId);

        //public Task SkipBackInQueue();

        //public Task SkipForwardInQueue(int index = 0);

        //public Task AddSongsToQueue(SongsToPlaylist request);

        //public Task ClearQueue();

        //public Task RemoveSongsInQueue(SongsToRemove request);

        // Creates a session and returns the sessionID aka the group name to the creator
        public async Task CreateSession()
        {
            //var newGuid = Guid.NewGuid();

            //if (!(await this.streamingService.CreateGroupAsync(newGuid.ToString())))
            //{
            //    newGuid = Guid.NewGuid();
            //    await this.streamingService.CreateGroupAsync(newGuid.ToString());
            //}

            //await this.Groups.AddToGroupAsync(Context.ConnectionId, newGuid.ToString());
            //await this.Clients.Caller.GetSessionId(newGuid);
        }


    }
}
