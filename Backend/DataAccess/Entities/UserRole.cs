using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class UserRole : IdentityUserRole<long>
    {
        public DateTime Created { get; set; }

        public User User { get; set; }

        public Role Role { get; set; }
    }
}
