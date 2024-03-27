using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class GroupQueueEntity
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public Guid GroupId { get; set; }

        public Song Song { get; set; }

        public bool AddedManualy { get; set; }
    }
}
