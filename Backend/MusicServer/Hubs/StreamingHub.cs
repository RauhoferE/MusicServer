using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MusicServer.Core.Const;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.HubEntities;
using MusicServer.Entities.Requests.Song;
using MusicServer.Interfaces;
using MusicServer.Services;
using Serilog;
using System;
using System.ComponentModel.DataAnnotations;

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

        public async Task CreateSession()
        {
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
            await this.Clients.Caller.ReceiveGroupName(newGuid);

            // Get the queue and queue data from the queue service
            var queueData = await this.queueService.GetQueueDataEntityAsync();
            var queueEntities = await this.queueService.GetAllQueueEntitiesAsync();

            // set the queue data as the group queue data and return it to the caller
            if (queueData != null)
            {
                queueData.UserId = this.activeUserService.Id;
                await this.groupQueueService.SetQueueDataAsync(newGuid, queueData);
                var data = await this.groupQueueService.GetQueueDataAsync(newGuid);
                await this.Clients.Caller.ReceiveQueueData(data);
            }

            // set the queue entities as the group queue entities and return the entities
            // and the current song to the caller
            if (queueEntities.Length > 0)
            {
                await this.groupQueueService.SetQueueEntitiesAsync(newGuid, queueEntities);
                var queueSongs = await this.groupQueueService.GetCurrentQueueAsync(newGuid);
                queueSongs = await this.groupQueueService.MarkQueueSongsAsFavorite(this.activeUserService.Id, queueSongs);
                var currentSong = await this.groupQueueService.GetCurrentSongInQueueAsync(newGuid);
                currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);
                await this.Clients.Caller.ReceiveQueueEntities(queueSongs);
                await this.Clients.Caller.ReceiveCurrentPlayingSong(currentSong);
            }
        }

        public async Task JoinSession(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Group doesnt exist!");
            }

            // Check if user is already part of a group
            if (await this.streamingService.IsUserPartOfGroup(this.activeUserService.Id))
            {
                throw new HubException("You have to leave the current session to join!");
            }

            // Delete the group with only the user as client
            //var gName = await this.streamingService.GetGroupName(this.Context.ConnectionId);
            //await this.streamingService.DeleteGroupAsync(gName);
            //await this.groupQueueService.RemoveQueueDataAndEntitiesAsync(gName);
            //await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, gName.ToString());

            // Join the new group
            if (!(await this.streamingService.JoinGroup(groupId, this.activeUserService.Id, this.Context.ConnectionId, this.activeUserService.Email)))
            {
                throw new HubException("Unknown Error when joining group!");
            }

            await this.Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());

            var userList = await this.streamingService.GetEmailList(groupId);
            var queueData = await this.groupQueueService.GetQueueDataAsync(groupId);
            var currentSong = await this.groupQueueService.GetCurrentSongInQueueAsync(groupId);
            currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);

            // Send the new user the groupname, userlist, current song, and queue data
            await this.Clients.Caller.ReceiveGroupName(groupId);
            await this.Clients.Caller.ReceiveUserList(userList.Where(x => x != this.activeUserService.Email).ToArray());
            await this.Clients.Caller.ReceiveQueueData(queueData);
            await this.Clients.Caller.ReceiveCurrentPlayingSong(currentSong);
            
            // Send other people a message that the user joined
            await this.Clients.GroupExcept(groupId.ToString(), this.Context.ConnectionId).UserJoinedSession(this.activeUserService.Email);
        }

        public async Task SendCurrentSongProgress(Guid groupId, bool isSongPlaying, double secondsPlayed)
        {
            // Maybe replace with filter
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.Clients.GroupExcept(groupId.ToString(), this.Context.ConnectionId).ReceiveSongProgress(isSongPlaying, secondsPlayed);
        }

        public async Task GetCurrentSongQueue(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var queue = await this.groupQueueService.GetCurrentQueueAsync(groupId);
            queue = await this.groupQueueService.MarkQueueSongsAsFavorite(this.activeUserService.Id, queue);
            await this.Clients.Caller.ReceiveQueueEntities(queue);
        }

        public async Task SkipBackInQueue(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            
            var currentSong = await this.groupQueueService.SkipBackInQueueAsync(groupId);
            //currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);


            //await this.Clients.Group(groupId.ToString()).ReceiveCurrentPlayingSong(currentSong);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task SkipForwardInQueue(Guid groupId, int index = 0)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            PlaylistSongDto currentSong = null;

            if (index < 1)
            {
                currentSong = await this.groupQueueService.SkipForwardInQueueAsync(groupId);
            }


            if (index > 0)
            {
                currentSong = await this.groupQueueService.SkipForwardInQueueAsync(groupId, index);
            }

            //if (currentSong != null)
            //{
            //    currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);
            //}

            //await this.Clients.Group(groupId.ToString()).ReceiveCurrentPlayingSong(currentSong);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task ClearQueue(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.groupQueueService.ClearManuallyAddedQueueAsync(groupId);
            //var queue = await this.groupQueueService.GetCurrentQueueAsync(groupId);
            //queue = await this.groupQueueService.MarkQueueSongsAsFavorite(this.activeUserService.Id, queue);    
            //await this.Clients.Caller.ReceiveQueueEntities(queue);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public async Task AddSongsToQueue(Guid groupId, Guid[] songIds)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.groupQueueService.AddSongsToQueueAsync(groupId, songIds);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public async Task RemoveSongsInQueue(Guid groupId, int[] orderIds)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.groupQueueService.RemoveSongsWithIndexFromQueueAsync(groupId, orderIds);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public async Task PushSongInQueue(Guid groupId, int srcIndex, int targetIndex, int markAsAddedManually = -1)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            await this.groupQueueService.PushSongToIndexAsync(groupId, srcIndex, targetIndex, markAsAddedManually);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public async Task GetCurrentSong(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var currentSong = await this.groupQueueService.GetCurrentSongInQueueAsync(groupId);
            currentSong = await this.groupQueueService.MarkPlaylistSongAsFavorite(this.activeUserService.Id, currentSong);
            await this.Clients.Caller.ReceiveCurrentPlayingSong(currentSong);
        }

        public async Task GetQueueData(Guid groupId)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var data = await this.groupQueueService.GetQueueDataAsync(groupId);
            await this.Clients.Caller.ReceiveQueueData(data);
        }

        public async Task UpdateLoopMode(Guid groupId, string loopMode)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var data = await this.groupQueueService.GetQueueDataAsync(groupId);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, data.ItemId, loopMode, data.SortAfter, data.Target, data.Random, data.Asc, data.UserId);
            data = await this.groupQueueService.GetQueueDataAsync(groupId);
            await this.Clients.Group(groupId.ToString()).ReceiveQueueData(data);
        }

        public async Task RandomizeQueue(Guid groupId, bool randomize = false)
        {
            if (!(await this.streamingService.GroupExistsAsync(groupId)))
            {
                throw new HubException("Error group doesnt exist.");
            }

            var queueEntitiy = await this.groupQueueService.GetQueueDataAsync(groupId);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, queueEntitiy.ItemId, queueEntitiy.LoopMode, queueEntitiy.SortAfter, queueEntitiy.Target, randomize, queueEntitiy.Asc, queueEntitiy.UserId);
            var queueData = await this.groupQueueService.GetQueueDataAsync(groupId);
            await this.Clients.Group(groupId.ToString()).ReceiveQueueData(queueData);
            if (queueData.Target == QueueTarget.Favorites)
            {
                // Get favroites
                var favoriteSongCount = await this.playlistService.GetFavoriteSongCountAsync(queueData.UserId);
                var favorites = await this.playlistService.GetFavoritesOfUserAsync(queueData.UserId, 0, favoriteSongCount, null, true, null);
                await this.groupQueueService.ChangeQueueAsync(groupId, favorites.Songs, randomize);
            }

            if (queueData.Target == QueueTarget.Playlist)
            {
                // Get playlist
                var playlistSongCount = await this.playlistService.GetPlaylistSongCountAsync(queueEntitiy.ItemId);
                var playlistSongs = await this.playlistService.GetSongsInPlaylistOfUserAsync(queueData.UserId, queueEntitiy.ItemId, 0, playlistSongCount, null, true, null);
                await this.groupQueueService.ChangeQueueAsync(groupId, playlistSongs.Songs, randomize);
            }

            if (queueData.Target == QueueTarget.Album)
            {
                // Get Album songs
                var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(queueEntitiy.ItemId);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(queueEntitiy.ItemId, 0, albumSongCount);
                await this.groupQueueService.ChangeQueueAsync(groupId, albumSongs.Songs, randomize);
            }

            if (queueData.Target == QueueTarget.Song)
            {
                var songDetails = await this.songService.GetSongInformationAsync(queueEntitiy.ItemId);
                var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(songDetails.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(songDetails.Album.Id, 0, albumSongCount);
                await this.groupQueueService.ChangeQueueAsync(groupId, albumSongs.Songs, randomize);
            }

            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task CreateQueueFromAlbum(Guid groupId, Guid albumId, bool randomize, string loopMode, int playFromIndex = 0)
        {
            var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(albumId);
            var albumSongs = await this.songService.GetSongsInAlbumAsync(albumId, 0, albumSongCount);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, albumId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Album, randomize, true, this.activeUserService.Id);
            await this.groupQueueService.CreateQueueAsync(groupId, albumSongs.Songs, randomize, playFromIndex);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task CreateQueueFromPlaylist(Guid groupId, Guid playlistId, bool randomize, string loopMode, string sortAfter = null, bool asc = true, int playFromOrder = 0)
        {
            var playlistSongCount = await this.playlistService.GetPlaylistSongCountAsync(playlistId);
            var playlistSongs = await this.playlistService.GetSongsInPlaylistOfUserAsync(this.activeUserService.Id, playlistId, 0, playlistSongCount, sortAfter, asc, null);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, playlistId, loopMode, sortAfter, QueueTarget.Playlist, randomize, asc, this.activeUserService.Id);
            await this.groupQueueService.CreateQueueAsync(groupId, playlistSongs.Songs, randomize, playFromOrder);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task CreateQueueFromFavorites(Guid groupId, bool randomize, string loopMode, string sortAfter = null, bool asc = true, int playFromOrder = 0)
        {
            var favoriteSongCount = await this.playlistService.GetFavoriteSongCountAsync(this.activeUserService.Id);
            var favoriteSongs = await this.playlistService.GetFavoritesOfUserAsync(this.activeUserService.Id, 0, favoriteSongCount, sortAfter, asc, null);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, Guid.Empty, loopMode, sortAfter, QueueTarget.Favorites, randomize, asc, this.activeUserService.Id);
            await this.groupQueueService.CreateQueueAsync(groupId, favoriteSongs.Songs, randomize, playFromOrder);
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
            await this.Clients.Group(groupId.ToString()).UpdateCurrentSong();
        }

        public async Task CreateQueueFromSingleSong(Guid groupId, Guid songId, bool randomize, string loopMode)
        {
            var song = await this.songService.GetSongInformationAsync(songId);
            await this.groupQueueService.UpdateQueueDataAsync(groupId, songId, loopMode, SortingElementsOwnPlaylistSongs.Name, QueueTarget.Song, randomize, true, this.activeUserService.Id);

            if (randomize)
            {
                var albumCount = await this.songService.GetSongCountOfAlbumAsync(song.Album.Id);
                var albumSongs = await this.songService.GetSongsInAlbumAsync(song.Album.Id, 0, albumCount);
                albumSongs.Songs = albumSongs.Songs.Where(x => x.Id != songId).Prepend(song).ToArray();
                await this.groupQueueService.CreateQueueAsync(groupId, albumSongs.Songs, randomize, 0);
            }

            await this.groupQueueService.CreateQueueAsync(groupId, new[] { song }, false, -1);
        }

        public async Task AddAlbumToQueue(Guid groupId, Guid albumId)
        {
            var albumSongCount = await this.songService.GetSongCountOfAlbumAsync(albumId);
            var albumSongs = await this.songService.GetSongsInAlbumAsync(albumId, 0, albumSongCount);
            await this.groupQueueService.AddSongsToQueueAsync(groupId, albumSongs.Songs.Select(x => x.Id).ToArray());
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public async Task AddPlaylistToQueue(Guid groupId, Guid playlistId)
        {
            var playlistSongCount = await this.playlistService.GetPlaylistSongCountAsync(playlistId);
            var playlistSongs = await this.playlistService.GetSongsInPlaylistAsync(playlistId, 0, playlistSongCount, "name", true, null);
            await this.groupQueueService.AddSongsToQueueAsync(groupId, playlistSongs.Songs.Select(x => x.Id).ToArray());
            await this.Clients.Group(groupId.ToString()).UpdateQueueView();
        }

        public override async Task OnConnectedAsync()
        {
            Log.Information($"User connected {this.Context.ConnectionId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Log.Information($"User disconnected {this.Context.ConnectionId}");

            // When a user disconnects remove him from all groups and all entities related to that user
            // If the user is master remove the group and signal the group that the group has been deleted and remove him from the group.
            // If the user was not a master just signal the group that the user left. And remove him from the group
            // The rest is done in the frontend

            if (exception == null)
            {
                Log.Warning($"User disconnected unexpectatly {this.Context.ConnectionId}");
            }

            var groupId = await this.streamingService.GetGroupName(this.Context.ConnectionId);

            // IF the user didn't had a group just return
            if (Guid.Empty == groupId)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            // Remove User from group
            var resp = await this.streamingService.DeleteUserWithConnectionId(this.Context.ConnectionId);

            // Not necessary
            //await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupId.ToString());

            // Send info that user disconnected
            await this.Clients.Group(groupId.ToString()).UserDisconnected(resp.Email);

            if (!resp.IsMaster)
            {
                await base.OnDisconnectedAsync(exception);
                return;
            }

            // If User is master remove all other users from group
            // And send notification that the group has been deleted
            await this.streamingService.DeleteGroupAsync(groupId);
            await this.groupQueueService.RemoveQueueDataAndEntitiesAsync(groupId);

            await this.Clients.Group(groupId.ToString()).GroupDeleted();

            await base.OnDisconnectedAsync(exception);
        }
    }
}
