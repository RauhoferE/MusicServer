using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class AllFollowedEntitiesResponse
    {
        public UserDto[] FollowedUsers { get; set; }

        public GuidNameDto[] FollowedArtists { get; set; }

        public FollowedPlaylistDto[] FollowedPlaylists { get; set; }

        public int FavoritesSongCount { get; set; }
    }
}
