namespace MusicServer.Entities.DTOs
{
    public class UserDetailsDto
    {// This probably needs changing 30.06.2023
        public long Id {get; set; }

        public string UserName { get; set; }


        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool IsDeleted { get; set; }

        public bool DemandPasswordChange { get; set; }

        public string? TemporarayEmail { get; set; }

        public string Email { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public LongNameDto[] FollowedUsers { get; set; }

        public GuidNameDto[] FollowedArtists { get; set; }

        public PlaylistUserShortDto[] Playlists { get; set; }

        public LongNameDto[] Roles { get; set; }
    }
}
