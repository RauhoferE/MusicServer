using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Interfaces
{
    public interface IMessageService
    {
        public Task ArtistAddedMessage(Guid artistId);

        public Task ArtistSongsAddedMessage(Guid artistId, List<Guid> songIds);
    }
}
