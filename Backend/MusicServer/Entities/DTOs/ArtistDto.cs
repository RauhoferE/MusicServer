namespace MusicServer.Entities.DTOs
{
    public class ArtistDto
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public AlbumArtistDto[] Albums { get; set; }

        public SongArtistDto[] Songs { get; set; }
    }
}
