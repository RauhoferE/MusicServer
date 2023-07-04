using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class MessageSongId
    {
        public long Id { get; set; }

        public Message Message { get; set; }

        public Guid SongId { get; set; }
    }
}
