using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Entities.DTOs;
using MusicServer.Interfaces;
using System.Collections.Concurrent;
using System.IO;

namespace MusicServer.Hubs
{
    //[Authorize]
    public class StreamingHub : Hub
    {
        // Possible params if the user wants to have a public session
        //private readonly ConcurrentDictionary<string, List<string>> SessionParameters;

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

        public async Task CreateSession()
        {
            if (this.Sessions.ContainsKey(this.Context.ConnectionId))
            {
                return;
            }

            this.Sessions.TryAdd(this.Context.ConnectionId, new List<string>());
        }

        public async Task<string[]> GetCurrentSession()
        {
            return this.Sessions.Keys.ToArray();
        }

        public async Task JoinSessionRequest(string leaderId)
        {
            await this.Clients.User(leaderId).SendCoreAsync("JoinRequest", new string[1]
            {
                this.Context.ConnectionId,
            } );
        }

        public async Task AllowJoin(string joinId)
        {
            var oldListenerList = this.Sessions.GetValueOrDefault(this.Context.ConnectionId);

            if (oldListenerList == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(oldListenerList.FirstOrDefault(joinId)))
            {
                return;
            }

            oldListenerList.Add(joinId);

            this.Sessions.TryUpdate(this.Context.ConnectionId, oldListenerList, this.Sessions.GetValueOrDefault(this.Context.ConnectionId));

            await this.Clients.User(joinId).SendAsync("JoinSuccess");
        }


    }
}
