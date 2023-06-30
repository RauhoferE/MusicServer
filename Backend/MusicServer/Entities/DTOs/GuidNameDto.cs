namespace MusicServer.Entities.DTOs
{
    public class GuidNameDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool FollowedByUser { get; set; } = false;

        public bool ReceiveNotifications { get; set; } = false;
    }
}
