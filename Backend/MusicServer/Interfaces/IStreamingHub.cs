using Microsoft.AspNetCore.Mvc;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.HubEntities;
using MusicServer.Entities.Requests.Song;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Interfaces
{
    public interface IStreamingHub
    {
        // Returns to the user who created it a unique Id
        public Task GetGroupName(Guid id);

        public Task UserJoinedSession(string email);

        public Task UserDisconnected(string email);

        public Task GroupDeleted();

        public Task GetPlayerData(CurrentPlayerData playerData);

        public Task GetQueue(QueueSongDto[] songs);

        public Task GetCurrentPlayingSong(PlaylistSongDto song);
    }
}
