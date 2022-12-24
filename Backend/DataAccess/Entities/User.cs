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
        public string Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool  IsDeleted { get; set; }

        public bool DemandPasswordChange { get; set; }

        public List<Artist> FollowedArtists { get; set; }

        public List<Song> Favorites { get; set; }

        public List<User> FollowedUsers { get; set; }

        public List<PlaylistUser> Playlists { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
