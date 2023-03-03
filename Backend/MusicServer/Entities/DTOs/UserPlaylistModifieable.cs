namespace MusicServer.Entities.DTOs
{
    public class UserPlaylistModifieable
    {
        public long UserId { get; set; }

        public bool CanModify { get; set; }
    }
}
