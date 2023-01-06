using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IActiveUserService
    {
        string Email { get; }
        Guid Id { get; }
        bool IsNull { get; }
        List<string> Roles { get; }
    }
}
