namespace MusicServer.Entities.DTOs
{
    public class PlaylistUserShortDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UserDto[] Users { get; set; }

        public int SongCount { get; set; }

        public bool IsModifieable { get; set; }

        public bool ReceiveNotifications { get; set; }

        public int Order { get; set; }

        public bool IsPublic { get; set; }  
    }
}
