﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    // Test
    public abstract class BaseEntity
    {
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Name { get; set; }    
    }
}
