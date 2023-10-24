namespace MusicServer.Entities.DTOs
{
    public class PlaylistSongDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public double Duration { get; set; }

        public AlbumArtistDto Album { get; set; }

        public ArtistShortDto[] Artists { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public int Order { get; set; }

        public bool IsInFavorites { get; set; } = false;
    }
}
