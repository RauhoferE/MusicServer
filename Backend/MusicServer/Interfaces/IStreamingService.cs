using DataAccess.Entities;
using MusicServer.Entities.HubEntities;

namespace MusicServer.Interfaces
{
    public interface IStreamingService
    {
        public Task<bool> CreateGroupAsync(Guid id, string userId, string connectionId);

        public Task<bool> JoinGroup(Guid id, string userId, string connectionId);

        public Task<bool> IsUserAlreadyInGroupAsync(string userId, bool isMaster);

        // This checks if the user isnt already part of a group with atleast 1 other member
        public Task<bool> CanUserJoinGroup(string userId);

        public Task<bool> GroupExistsAsync(Guid groupId);

        public Task<DeleteGroupResponse[]> DeleteGroupAsync(Guid groupId);

        public Task<string> GetConnectionIdOfUser(string email, Guid groupId);

        public Task<Guid> GetGroupName(string connectionId);

        public Task<RemoveUserResponse> DeleteUser(string userId);

        public Task<RemoveUserResponse> DeleteUserWithConnectionId(string connectionId);

        public Task<string> GetConnectionIdOfMaster(Guid groupId);

        public Task<long> GetIdOfMaster(Guid groupId);
    }
}
