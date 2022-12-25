using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? Description { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime Modified { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool DemandPasswordChange { get; set; } = false;

        public List<Artist> FollowedArtists { get; set; } = new List<Artist>();

        public List<Song> Favorites { get; set; } = new List<Song>();

        public List<User> FollowedUsers { get; set; } = new List<User>();

        public List<PlaylistUser> Playlists { get; set; } = new List<PlaylistUser>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
