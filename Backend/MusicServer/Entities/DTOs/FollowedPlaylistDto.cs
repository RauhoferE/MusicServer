namespace MusicServer.Entities.DTOs
{
    public class FollowedPlaylistDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int SongCount { get; set; }

        public string CreatorName { get; set; }
    }
}
