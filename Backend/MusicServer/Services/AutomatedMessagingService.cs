using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using Org.BouncyCastle.Utilities;
using Renci.SshNet.Messages;
using Serilog;

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
            var messageType = this.dBContext.LovMessageTypes
    .FirstOrDefault(x => x.Id == (long)Core.Const.MessageType.PlaylistSongsAdded) ??
    throw new MessageTypeNotFoundException();

            var alreadyExistingMessage = this.dBContext.MessageQueue
                .Include(x => x.Type)
                .Include(x => x.Songs)
                .FirstOrDefault(x => x.UserId == userId && x.Type.Id == messageType.Id && x.PlaylistId == playlistId); 

            var messageSongIds = songIds.Take(10).Select(x => new MessageSongId()
            {
                SongId = x
            }).ToList();

            if (alreadyExistingMessage == null)
            {
                this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
                {
                    ArtistId = Guid.Empty,
                    PlaylistId = playlistId,
                    UserId = userId,
                    Songs = messageSongIds,
                    Type = messageType
                });

                await this.dBContext.SaveChangesAsync();
                return;
            }

            alreadyExistingMessage.Songs = alreadyExistingMessage.Songs.Concat(messageSongIds).ToList();
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
                .Include(x => x.Songs)
                .Include(x => x.Type).ToList();
            
            var messageGroups = messages.GroupBy(x => x.Type.Id, y => y);

            foreach (var item in messageGroups)
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
                        await this.PreparePlaylistSharedEmail(item, false);
                        break;
                    case Core.Const.MessageType.PlaylistShareRemoved:
                        await this.PreparePlaylistSharedEmail(item, true);
                        break;
                    case Core.Const.MessageType.ArtistTracksAdded:
                        await this.PrepareArtistTracksAddedEmail(item);
                        break;
                    case Core.Const.MessageType.ArtistAdded:
                        await this.PrepareArtistAddedEmail(item);
                        break;
                    default:
                        break;
                }
            }

            this.dBContext.MessageQueue.RemoveRange(messages);
            await this.dBContext.SaveChangesAsync();
        }

        private async Task PrepareArtistAddedEmail(IGrouping<long, DataAccess.Entities.Message> messages)
        {
            var users = this.dBContext.Users.ToList();
            var artistIds = messages.Select(x => x.ArtistId).ToList();
            var artists = this.dBContext.Artists.Where(x => artistIds.Contains(x.Id)).Take(50).ToList();

            if (users.Count() == 0 || artists.Count() == 0)
            {
                return;
            }

            foreach (var user in users)
            {
                await this.mailService.SendNewArtistsAddedEmail(user, artists);
            }
        }

        private async Task PrepareArtistTracksAddedEmail(IGrouping<long, DataAccess.Entities.Message> messages)
        {
            foreach (var message in messages)
            {
                var followedUser = this.dBContext.FollowedArtists
                    .Include(x => x.User)
                    .Include(x => x.Artist)
                    .Where(x => x.Artist.Id == message.ArtistId)
                    .Select(x => x.User);

                var artist = this.dBContext.Artists.FirstOrDefault(x => x.Id == message.ArtistId);

                var songs = this.dBContext.Songs.Where(x =>
                            message.Songs.Select(y => y.SongId).Contains(x.Id))
                                .Take(10).ToList();

                if (followedUser.Count() == 0 || artist == null || songs.Count() == 0)
                {
                    continue;
                }

                foreach (var user in followedUser)
                {
                    await this.mailService.SendTracksAddedFromArtistEmail(user, artist, songs);
                }
            }
        }

        private async Task PreparePlaylistSharedEmail(IGrouping<long, DataAccess.Entities.Message> messages, bool removeUser)
        {
            foreach (var message in messages)
            {
                var sharedUser = this.dBContext.Users.FirstOrDefault(x => x.Id == message.TargetUserId);

                var playlist = this.dBContext.Playlists.FirstOrDefault(x => x.Id == message.PlaylistId);

                var user = this.dBContext.Users.FirstOrDefault(x => x.Id == message.UserId);

                if (sharedUser == null || playlist == null || user == null)
                {
                    continue;
                }

                if (removeUser)
                {
                    await this.mailService.SendPlaylistRemovedFromUserEmail(user, playlist, sharedUser);
                    continue;
                }

                await this.mailService.SendPlaylistSharedWithUserEmail(user, playlist, sharedUser);
            }
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
