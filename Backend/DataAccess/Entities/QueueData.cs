﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class QueueData
    {
        public int Id { get; set; }

        public bool Asc { get; set; }

        public bool Random { get; set; }

        public long UserId { get; set; }

        public LovPlaylistSortAfter SortAfter { get; set; }

        public LovQueueTarget Target { get; set; }

        public LovLoopMode LoopMode { get; set; }

        public Guid ItemId { get; set; }
    }
}
