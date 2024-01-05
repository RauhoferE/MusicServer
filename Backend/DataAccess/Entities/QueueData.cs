using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class QueueData
    {
        public int Id { get; set; }

        public bool Asc { get; set; }

        public bool Random { get; set; }

        public long UserId { get; set; }

        public LutQueueTarget Target { get; set; }

        public LutLoopMode LoopMode { get; set; }

        public Guid ItemId { get; set; }
    }
}
