namespace MusicServer.Entities.DTOs
{
    public class PlaylistShortDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int SongCount { get; set; }
    }
}
