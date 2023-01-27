namespace MusicServer.Entities.DTOs
{
    public class PlaylistDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime? LastListened { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public PlaylistUserDto[] Users { get; set; } 
    }
}
