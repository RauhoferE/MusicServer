using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    { 
        public Role Role { get; set; }
    }
}
