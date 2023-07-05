using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Renci.SshNet.Messages;
using static MusicServer.Const.ApiRoutes;

namespace MusicServer.Services
{
    public class AutomatedMessagingService : IAutomatedMessagingService
    {
        private readonly MusicServerDBContext dBContext;
        private readonly IMusicMailService mailService;

        public AutomatedMessagingService(MusicServerDBContext dBContext, IMusicMailService mailService)
        {
            this.mailService = mailService;
            this.dBContext = dBContext;
        }

        public async Task AddPlaylistMessage(long userId, Guid playlistId)
        {
            var messageType = this.dBContext.LovMessageTypes
                .FirstOrDefault(x => x.Id == (long)Core.Const.MessageType.PlaylistAdded) ??
                throw new MessageTypeNotFoundException();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = Guid.Empty,
                PlaylistId = playlistId,
                UserId = userId,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddSongsToPlaylistMessage(long userId, Guid playlistId, List<Guid> songIds)
        {
            // TODO: Check if similar message already exists
            var messageType = this.dBContext.LovMessageTypesand append
    .FirstOrDefault(x => x.Id == (long)Core.Const.MessageType.PlaylistSongsAdded) ??
    throw new MessageTypeNotFoundException();

            var messageSongIds = songIds.Select(x => new MessageSongId()
            {
                SongId = x
            }).ToList();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = Guid.Empty,
                PlaylistId = playlistId,
                UserId = userId,
                Songs = messageSongIds,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task PlaylistRemoveMessage(long userId, Guid playlistId, long targetUserId)
        {
            var messageType = this.dBContext.LovMessageTypes
            .FirstOrDefault(x => x.Id == (long)Core.Const.MessageType.PlaylistShareRemoved) ??
            throw new MessageTypeNotFoundException();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = Guid.Empty,
                PlaylistId = playlistId,
                UserId = userId,
                TargetUserId = targetUserId,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task PlaylistShareMessage(long userId, Guid playlistId, long targetUserId)
        {
            var messageType = this.dBContext.LovMessageTypes
.FirstOrDefault(x => x.Id == (long)Core.Const.MessageType.PlaylistShared) ??
throw new MessageTypeNotFoundException();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = Guid.Empty,
                PlaylistId = playlistId,
                UserId = userId,
                TargetUserId = targetUserId,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task SendAutomatedMessages()
        {
            var messages = this.dBContext.MessageQueue
                .Include(x => x.Type).ToList().GroupBy(x => x.Type.Id, y => y);

            foreach (var item in messages)
            {
                switch ((Core.Const.MessageType)item.Key)
                {
                    case Core.Const.MessageType.PlaylistAdded:
                        await this.PrepareAddPlaylistEmail(item);
                        break;
                    case Core.Const.MessageType.PlaylistSongsAdded:
                        await this.PreparePlaylistSongsAddedEmail(item);
                        break;
                    case Core.Const.MessageType.PlaylistShared:
                        await this.PreparePlaylistSharedEmail(item);
                        break;
                    case Core.Const.MessageType.PlaylistShareRemoved:
                        break;
                    case Core.Const.MessageType.ArtistTracksAdded:
                        break;
                    case Core.Const.MessageType.ArtistAdded:
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task PreparePlaylistSharedEmail(IGrouping<long, DataAccess.Entities.Message> item)
        {
            throw new NotImplementedException();
        }

        private async Task PreparePlaylistSongsAddedEmail(IGrouping<long, DataAccess.Entities.Message> messages)
        {
            foreach (var message in messages)
            {
                var followedUsers = this.dBContext.PlaylistUsers
                    .Include(x => x.User)
                    .Include(x => x.Playlist)
                    .Where(x => x.ReceiveNotifications && x.User.Id != message.UserId && 
                    x.Playlist.Id == message.PlaylistId)
                    .Select(x => x.User);

                var playlist = this.dBContext.Playlists.FirstOrDefault(x => x.Id == message.PlaylistId);

                var user = this.dBContext.Users.FirstOrDefault(x => x.Id == message.UserId);

                var songs = this.dBContext.Songs.Where(x => 
                message.Songs.Select(y => y.SongId).Contains(x.Id))
                    .Take(10).ToList();

                if (followedUsers.Count() == 0 || playlist == null || user == null || songs.Count() == 0)
                {
                    continue;
                }

                foreach (var followedUser in followedUsers)
                {
                    await this.mailService.SendTracksAddedToPlaylistEmail(user, playlist, songs, followedUser);
                }
            }
        }

        private async Task PrepareAddPlaylistEmail(IGrouping<long, DataAccess.Entities.Message> messages)
        {
            foreach (var message in messages)
            {
                var followedUsers = this.dBContext.FollowedUsers
                    .Include(x => x.User)
                    .Include(x => x.FollowedUser)
                    .Where(x => x.ReceiveNotifications && x.FollowedUser.Id == message.UserId)
                    .Select(x => x.User);

                var playlist = this.dBContext.Playlists.FirstOrDefault(x => x.Id == message.PlaylistId);

                var user = this.dBContext.Users.FirstOrDefault(x => x.Id == message.UserId);

                if (followedUsers.Count() == 0 || playlist == null || user == null)
                {
                    continue;
                }

                foreach (var followedUser in followedUsers)
                {
                    await this.mailService.SendPlaylistAddedFromUserEmail(user, playlist, followedUser);
                }
            }
        }
    }
}
