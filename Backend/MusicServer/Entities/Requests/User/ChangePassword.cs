namespace MusicServer.Entities.Requests.User
{
    public class ChangePassword
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
