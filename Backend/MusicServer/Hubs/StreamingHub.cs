using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Interfaces;
using System.IO;

namespace MusicServer.Hubs
{
    //[Authorize]
    public class StreamingHub : Hub
    {
        public async IAsyncEnumerable<byte> StreamSong(Guid songId, IFileService fileService)
        {
            await foreach (var item in fileService.GetSongStream(songId))
            {
                yield return item;
            }
        }

        public async Task SendMessage(string user, string message)
    => await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
