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

        public bool IsModifieable { get; set; } = true;

        public bool IsCreator { get; set; } = false;

        public bool ReceiveNotifications { get; set; } = true;

        public DateTime? LastListened { get; set; }

        public int Order { get; set; }
    }
}
