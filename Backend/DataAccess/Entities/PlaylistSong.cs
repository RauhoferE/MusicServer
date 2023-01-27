using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PlaylistSong : CompositeBaseEntity
    {
        //TODO: Add sorting id
        public Playlist Playlist { get; set; }

        public Song Song { get; set; }
    }
}
