using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class ArtistAlbum : CompositeBaseEntity
    {
        //public long ID { get; set; }    

        public Artist Artist { get; set; }

        public Album Album { get; set; }

        //public DateTime Added { get; set; }
    }
}
