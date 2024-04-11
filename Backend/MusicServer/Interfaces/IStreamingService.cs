using DataAccess.Entities;
using MusicServer.Entities.HubEntities;

namespace MusicServer.Interfaces
{
    public interface IStreamingService
    {
        public Task<bool> CreateGroupAsync(Guid id, long userId, string connectionId, string email);

        public Task<bool> GroupExistsAsync(Guid groupId);

        public Task<Guid> GetGroupName(string connectionId);

        public Task<RemoveUserResponse> DeleteUserWithConnectionId(string connectionId);

        public Task DeleteGroupAsync(Guid groupId);

        // This checks if the user isnt already part of a group with atleast 1 other member
        public Task<bool> CanUserJoinGroup(long userId);

        public Task<bool> IsUserPartOfGroup(long userId);

        public Task<bool> JoinGroup(Guid id, long userId, string connectionId, string email);

        public Task<SessionUserData[]> GetEmailList(Guid groupId);
    }
}
