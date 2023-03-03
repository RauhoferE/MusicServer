namespace MusicServer.Entities.Requests.User
{
    public class Register
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string UserName { get; set; }

        public DateTime Birth { get; set; }

        public Guid RegistrationCode { get; set; }
    }
}
