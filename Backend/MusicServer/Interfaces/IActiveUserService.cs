using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IActiveUserService
    {
        string Email { get; }
        long Id { get; }
        bool IsNull { get; }
        List<string> Roles { get; }
    }
}
