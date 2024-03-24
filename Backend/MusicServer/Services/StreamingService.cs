using DataAccess;
using MusicServer.Exceptions;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class StreamingService : IStreamingService
    {
        private readonly MusicServerDBContext dBContext;

        public StreamingService(MusicServerDBContext dBContext)
        {
            this.dBContext = dBContext;
                
        }

        public async Task<bool> CreateGroupAsync(Guid id, string userId, string connectionId)
        {
            if (await this.IsUserAlreadyInGroupAsync(userId))
            {
                return false;
            }

            if ((await this.GroupExistsAsync(id.ToString())))
            {
                return false;
            }

            this.dBContext.Groups.Add(new DataAccess.Entities.Group()
            {
                GroupName = id,
                IsMaster = true,
                UserId = long.Parse(userId),
                ConnectionId = connectionId
            });

            await this.dBContext.SaveChangesAsync();
            return true;
        }

        public async Task DeleteGroupAsync(string id)
        {
            var groups = this.dBContext.Groups.Where(x => x.GroupName == Guid.Parse(id));

            if (groups.Count() > 0)
            {
                this.dBContext.Groups.RemoveRange(groups);
                await this.dBContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUserFromGroup(string userId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.UserId == long.Parse(userId));

            if (group == null)
            {
                return;
            }

            if (group.IsMaster)
            {
                await this.DeleteGroupAsync(group.GroupName.ToString());
                return;
            }

            this.dBContext.Groups.Remove(group);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task<string> GetConnectionIdOfMaster(string groupId)
        {
            if (!(await this.GroupExistsAsync(groupId)))
            {
                throw new GroupNotFoundException($"Group with id: {groupId} not found!");
            }

            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == Guid.Parse(groupId) && x.IsMaster);
            return group?.ConnectionId;
        }

        public async Task<long> GetIdOfMaster(string groupId)
        {
            if (!(await this.GroupExistsAsync(groupId)))
            {
                throw new GroupNotFoundException($"Group with id: {groupId} not found!");
            }

            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == Guid.Parse(groupId) && x.IsMaster);
            return group.UserId;
        }

        public async Task<bool> GroupExistsAsync(string id)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == Guid.Parse(id) && x.IsMaster);
            return group != null;
        }

        public async Task<bool> IsUserAlreadyInGroupAsync(string userId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.UserId == long.Parse(userId));
            return group != null;
        }

        public async Task<bool> JoinGroup(Guid id, string connectionId, string userId)
        {
            if (await this.IsUserAlreadyInGroupAsync(userId))
            {
                return false;
            }

            if (!(await this.GroupExistsAsync(id.ToString())))
            {
                return false;
            }

            this.dBContext.Groups.Add(new DataAccess.Entities.Group()
            {
                GroupName = id,
                UserId = long.Parse(userId),
                ConnectionId = connectionId
            });

            await this.dBContext.SaveChangesAsync();
            return true;
        }
    }
}
