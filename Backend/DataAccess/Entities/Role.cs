using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public User Modifiedby { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<RoleClaim> RoleClaims { get; set; }
    }
}
