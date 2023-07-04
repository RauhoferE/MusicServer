using DataAccess;
using DataAccess.Entities;
using MusicServer.Core.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;

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
            throw new NotImplementedException();
        }
    }
}
