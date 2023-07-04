using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public enum MessageType
    {
        PlaylistAdded = 1,
        PlaylistSongsAdded = 2,
        PlaylistShared = 3,
        PlaylistShareRemoved = 4,
        ArtistTracksAdded = 5,
        ArtistAdded = 6
    }
}
