namespace MusicServer.Entities.Requests.User
{
    public class EditUser
    {
        public DateTime? LockoutEnd { get; set; }

        public bool DemandPasswordChange { get; set; }

        public bool IsDeleted { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
