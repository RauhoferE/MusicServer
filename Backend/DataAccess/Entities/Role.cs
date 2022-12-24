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
        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        public User? Modifiedby { get; set; }

        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public List<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
    }
}
