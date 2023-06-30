using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IMusicMailService
    {
        Task SendEmail(User user, string subject, string body);
    }
}
