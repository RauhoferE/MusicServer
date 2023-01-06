namespace MusicServer.Entities.DTOs
{
    public class PlaylistUserDto
    {
        public string UserName { get; set; }

        public Guid UserId { get; set; }

        public bool IsModifieable { get; set; }
    }
}
