using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Playlist : BaseEntity
    {
        public Guid ID { get; set; }

        public DateTime LastListened { get; set; }

        public bool IsPublic { get; set; }

        public string Description { get; set; }

        public List<PlaylistSong> Songs { get; set; }

        public List<PlaylistUser> Users { get; set; }
    }
}
