﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class User : IdentityUser<long>
    {
        public string? Description { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool DemandPasswordChange { get; set; } = false;

        public string TemporarayEmail { get; set; } = null;

        public ICollection<UserArtist> FollowedArtists { get; set; } = new List<UserArtist>();

        public ICollection<UserSong> Favorites { get; set; } = new List<UserSong>();

        public ICollection<UserUser> FollowedUsers { get; set; } = new List<UserUser>();

        public ICollection<PlaylistUser> Playlists { get; set; } = new List<PlaylistUser>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
