namespace MusicServer.Entities.DTOs
{
    public class FullUserDto
    {
        public long Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? LastLogin { get; set; }

        public DateTime Birth { get; set; }

        public bool IsDeleted { get; set; }

        public bool DemandPasswordChange { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public bool EmailConfirmed { get; set; }
    }
}
