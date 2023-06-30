namespace MusicServer.Entities.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public bool IsFollowedByUser { get; set; } = false; 

        public bool ReceiveNotifications { get; set; } = false;
    }
}
