namespace MusicServer.Entities.DTOs
{
    public class AlbumDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public GuidNameDto[] Artists { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int SongCount { get; set; }

        public double Duration { get; set; }
    }
}
