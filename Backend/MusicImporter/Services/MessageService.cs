using DataAccess;
using DataAccess.Entities;
using MusicImporter.Exceptions;
using MusicImporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Services
{
    public class MessageService : IMessageService
    {
        private readonly MusicServerDBContext dBContext;

        public MessageService(MusicServerDBContext dBContext)
        {
                this.dBContext = dBContext;
        }

        public async Task ArtistAddedMessage(Guid artistId)
        {
            var messageType = this.dBContext.LovMessageTypes
    .FirstOrDefault(x => x.Id == (long)MusicServer.Core.Const.MessageType.ArtistAdded) ??
    throw new NotFoundException();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = artistId,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }

        public async Task ArtistSongsAddedMessage(Guid artistId, List<Guid> songIds)
        {
            var messageType = this.dBContext.LovMessageTypes
.FirstOrDefault(x => x.Id == (long)MusicServer.Core.Const.MessageType.ArtistTracksAdded) ??
throw new NotFoundException();

            var messageSongIds = songIds.Select(x => new MessageSongId()
            {
                SongId = x
            }).ToList();

            this.dBContext.MessageQueue.Add(new DataAccess.Entities.Message()
            {
                ArtistId = artistId,
                Songs = messageSongIds,
                Type = messageType
            });

            await this.dBContext.SaveChangesAsync();
        }
    }
}
