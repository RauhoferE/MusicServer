using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class AllFollowedEntitiesResponse
    {
        public LongNameDto[] FollowedUsers { get; set; }

        public LongNameDto[] FollowedArtists { get; set; }

        public FollowedPlaylistDto[] FollowedPlaylists { get; set; }

        public int FavoritesSongCount { get; set; }
    }
}
