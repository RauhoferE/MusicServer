using DataAccess.Entities;
using MusicServer.Entities.HubEntities;

namespace MusicServer.Interfaces
{
    public interface IStreamingService
    {
        public Task<bool> CreateGroupAsync(Guid id, long userId, string connectionId, string email);

        public Task<bool> GroupExistsAsync(Guid groupId);






        public Task<bool> JoinGroup(Guid id, long userId, string connectionId, string email);

        public Task<bool> IsUserAlreadyInGroupAsync(string userId, bool isMaster);

        public Task<bool> IsUserAlreadyPartOfGroupWithOthers(string connectionId);

        public Task<string[]> GetEmailList(Guid groupId);

        // This checks if the user isnt already part of a group with atleast 1 other member
        public Task<bool> CanUserJoinGroup(string userId);

        public Task<DeleteGroupResponse[]> DeleteGroupAsync(Guid groupId);

        public Task<string> GetConnectionIdOfUser(string email, Guid groupId);

        public Task<Guid> GetGroupName(string connectionId);

        public Task<RemoveUserResponse> DeleteUser(string userId);

        public Task<RemoveUserResponse> DeleteUserWithConnectionId(string connectionId);

        public Task<string> GetConnectionIdOfMaster(Guid groupId);

        public Task<long> GetIdOfMaster(Guid groupId);
    }
}
