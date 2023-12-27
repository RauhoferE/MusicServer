using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class QueueEntity
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public long UserId { get; set; }

        public Song Song { get; set; }
    }
}
