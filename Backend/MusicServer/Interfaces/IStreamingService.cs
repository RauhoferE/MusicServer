namespace MusicServer.Interfaces
{
    public interface IStreamingService
    {
        public Task<bool> CreateGroupAsync(Guid id, string userId);

        public Task<bool> IsUserAlreadyInGroupAsync(string userId);

        public Task<bool> GroupExistsAsync(string id);

        public Task DeleteGroupAsync(string id);

        public Task DeleteUserFromGroup(string userId);

        public Task<string> GetConnectionIdOfMaster(string groupId);

        public Task<long> GetIdOfMaster(string groupId);
    }
}
