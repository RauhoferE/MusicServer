using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class AllFollowedEntitiesResponse
    {
        public UserDto[] FollowedUsers { get; set; }

        public ArtistShortDto[] FollowedArtists { get; set; }

        public FollowedPlaylistDto[] FollowedPlaylists { get; set; }

        public int FavoritesSongCount { get; set; }
    }
}
