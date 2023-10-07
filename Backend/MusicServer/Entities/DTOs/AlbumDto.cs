namespace MusicServer.Entities.DTOs
{
    public class AlbumDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ArtistShortDto[] Artists { get; set; }

        public DateTime Release { get; set; }

        public int SongCount { get; set; }

        public double Duration { get; set; }
    }
}
