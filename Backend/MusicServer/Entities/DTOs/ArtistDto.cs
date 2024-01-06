namespace MusicServer.Entities.DTOs
{
    public class ArtistDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }    

        public string? Description { get; set; }

        public bool FollowedByUser { get; set; } = false;

        public bool ReceiveNotifications { get; set; } = false;

        //public AlbumArtistDto[] Albums { get; set; }

        //public SongDto[] Songs { get; set; }
    }
}
