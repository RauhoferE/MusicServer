using DataAccess;
using DataAccess.Entities;
using MusicServer.Entities.HubEntities;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using System.Text.RegularExpressions;

namespace MusicServer.Services
{
    public class StreamingService : IStreamingService
    {
        private readonly MusicServerDBContext dBContext;

        public StreamingService(MusicServerDBContext dBContext)
        {
            this.dBContext = dBContext;
                
        }

        // The griup is created when a user join
        public async Task<bool> CreateGroupAsync(Guid id, long userId, string connectionId, string email)
        {
            if ((await this.GroupExistsAsync(id)))
            {
                return false;
            }

            this.dBContext.Groups.Add(new DataAccess.Entities.Group()
            {
                GroupName = id,
                IsMaster = true,
                UserId = userId,
                ConnectionId = connectionId,
                Email = email
            });

            await this.dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GroupExistsAsync(Guid id)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == id && x.IsMaster);
            return group != null;
        }

        public async Task<Guid> GetGroupName(string connectionId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.ConnectionId == connectionId);

            if (group == null)
            {
                return Guid.Empty;
            }

            return group.GroupName;
        }

        public async Task<RemoveUserResponse> DeleteUserWithConnectionId(string connectionId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.ConnectionId == connectionId);

            this.dBContext.Groups.Remove(group);
            await this.dBContext.SaveChangesAsync();
            return new RemoveUserResponse()
            {
                Email = this.dBContext.Users.First(x => x.Id == group.UserId).Email,
                IsMaster = group.IsMaster
            };
        }

        public async Task DeleteGroupAsync(Guid id)
        {
            var groups = this.dBContext.Groups.Where(x => x.GroupName == id);

            if (groups.Count() > 0)
            {
                var connectionIds = groups.Select(x => x.ConnectionId).ToArray();
                this.dBContext.Groups.RemoveRange(groups);
                await this.dBContext.SaveChangesAsync();
            }
        }

        public async Task<bool> CanUserJoinGroup(string userId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.UserId == long.Parse(userId)).GroupName;
            var groups = this.dBContext.Groups.Where(x => x.GroupName == group);
            return groups.Count() < 2;
        }

        public async Task<bool> JoinGroup(Guid id, long userId, string connectionId, string email)
        {
            if (!(await this.GroupExistsAsync(id)))
            {
                return false;
            }

            this.dBContext.Groups.Add(new DataAccess.Entities.Group()
            {
                GroupName = id,
                UserId = userId,
                ConnectionId = connectionId,
                Email = email
            });

            await this.dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<string[]> GetEmailList(Guid groupId)
        {
            return this.dBContext.Groups.Where(x => x.GroupName == groupId).Select(x => x.Email).ToArray();
        }
    }
}
