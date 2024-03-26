using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Group
    {
        public long Id { get; set; }

        public bool IsMaster { get; set; } = false;

        public Guid GroupName { get; set; }

        public long UserId { get; set; }

        public string Email { get; set; }

        public string ConnectionId { get; set; }
    }
}
