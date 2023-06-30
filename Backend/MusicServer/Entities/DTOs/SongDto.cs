namespace MusicServer.Entities.DTOs
{
    public class SongDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Length { get; set; }

        public AlbumArtistDto Album { get; set; }

        public GuidNameDto[] Artists { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool IsInFavorites { get; set; } = false;
    }
}
