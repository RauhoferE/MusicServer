using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PlaylistUser : CompositeBaseEntity
    {
        public Playlist Playlist { get; set; }

        public User User { get; set; }

        public bool IsModifieable { get; set; }
    }
}
