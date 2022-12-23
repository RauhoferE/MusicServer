using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Album : BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime Release { get; set; }


    }
}
