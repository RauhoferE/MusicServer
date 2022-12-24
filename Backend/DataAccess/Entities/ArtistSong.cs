using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class ArtistSong : CompositeBaseEntity
    {
        //public long ID { get; set; }

        public Artist Artist { get; set; }

        public Song Song { get; set; }

        //public DateTime Added { get; set; }
    }
}
