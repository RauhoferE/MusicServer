using DataAccess.Entities;
using MusicServer.Entities.HubEntities;

namespace MusicServer.Interfaces
{
    public interface IStreamingService
    {
        public Task<bool> CreateGroupAsync(Guid id, string userId, string connectionId);

        public Task<bool> JoinGroup(Guid id, string userId, string connectionId);

        public Task<bool> IsUserAlreadyInGroupAsync(string userId);

        // This checks if the user isnt already part of a group with atleast 1 other member
        public Task<bool> CanUserJoinGroup(string userId);

        public Task<bool> GroupExistsAsync(string id);

        public Task<string[]> DeleteGroupAsync(string id);

        public Task<string> GetGroupName(string connectionId);

        public Task<RemoveUserResponse> DeleteUser(string userId);

        public Task<RemoveUserResponse> DeleteUserWithConnectionId(string connectionId);

        public Task<string> GetConnectionIdOfMaster(string groupId);

        public Task<long> GetIdOfMaster(string groupId);
    }
}
