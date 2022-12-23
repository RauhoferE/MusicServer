using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Song : BaseEntity
    {
        public Guid Id { get; set; }

        public double Length { get; set; }

        // Missing Album, Artist
    }
}
