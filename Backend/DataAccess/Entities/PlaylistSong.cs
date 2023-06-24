using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PlaylistSong : CompositeBaseEntity
    {
        // The first element is always 0
        public int Order { get; set; }

        public Playlist Playlist { get; set; }

        public Song Song { get; set; }
    }
}
