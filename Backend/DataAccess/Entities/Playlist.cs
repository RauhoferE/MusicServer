﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Playlist : BaseEntity
    {
        public Guid Id { get; set; }

        public bool IsPublic { get; set; } = false;

        public string? Description { get; set; }

        public ICollection<PlaylistSong> Songs { get; set; }

        public ICollection<PlaylistUser> Users { get; set; }
    }
}
