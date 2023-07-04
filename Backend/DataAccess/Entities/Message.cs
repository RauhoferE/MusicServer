using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Message
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long TargetUserId { get; set; }

        public Guid ArtistId { get; set; }

        public Guid PlaylistId { get; set; }

        public MessageType Type { get; set; }

        public ICollection<MessageSongId> Songs { get; set; } = new List<MessageSongId>();
    }
}
