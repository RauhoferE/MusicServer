using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }

        //public string Name { get; set; }

        public string Email { get; set; }

        public string Description { get; set; }

        //public DateTime Created { get; set; }

        //public DateTime Modified { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool  IsDeleted { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool DemandPasswordChange { get; set; }

        // Missing Artists, Users, Songs, Playlists
    }
}
