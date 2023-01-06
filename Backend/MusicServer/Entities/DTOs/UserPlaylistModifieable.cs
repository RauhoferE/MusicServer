namespace MusicServer.Entities.DTOs
{
    public class UserPlaylistModifieable
    {
        public Guid UserId { get; set; }

        public bool CanModify { get; set; }
    }
}
