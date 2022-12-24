using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public abstract class CompositeBaseEntity
    {
        public long ID { get; set; }

        public DateTime Added { get; set; }
    }
}
