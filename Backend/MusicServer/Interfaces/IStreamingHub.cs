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
        public Task ReceiveGroupName(Guid id);

        public Task ReceiveQueueData(QueueDataDto queueDataDto);

        public Task ReceiveQueueEntities(QueueSongDto[] queueDataDto);

        public Task ReceiveCurrentPlayingSong(PlaylistSongDto song);

        public Task UserDisconnected(string email);

        public Task GroupDeleted();

        public Task ReceiveUserList(string[] userList);

        public Task UserJoinedSession(string email);

        public Task ReceiveSongProgress(bool isSongPlaying, double secondsPlayed);

        public Task UpdateQueueView();

        public Task UpdateCurrentSong();
    }
}
