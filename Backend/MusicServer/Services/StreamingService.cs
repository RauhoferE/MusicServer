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


        public async Task<bool> CanUserJoinGroup(string userId)
        {
            var group = this.dBContext.Groups.Where(x => x.UserId == long.Parse(userId));
            return group.Count() < 2;
        }



        public async Task<DeleteGroupResponse[]> DeleteGroupAsync(Guid id)
        {
            var groups = this.dBContext.Groups.Where(x => x.GroupName == id);

            if (groups.Count() > 0)
            {
                var connectionIds = groups.Select(x => new DeleteGroupResponse() {ConnectionId = x.ConnectionId, UserId = x.UserId, Email = x.Email }).ToArray();
                this.dBContext.Groups.RemoveRange(groups);
                await this.dBContext.SaveChangesAsync();
                return connectionIds;
            }

            return new DeleteGroupResponse[0];
        }

        public async Task<RemoveUserResponse> DeleteUser(string userId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.UserId == long.Parse(userId));

            this.dBContext.Groups.Remove(group);
            await this.dBContext.SaveChangesAsync();
            return new RemoveUserResponse()
            {
                Email = this.dBContext.Users.First(x => x.Id == group.UserId).Email,
                IsMaster = group.IsMaster
            };
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

        public async Task<string> GetConnectionIdOfMaster(Guid groupId)
        {
            if (!(await this.GroupExistsAsync(groupId)))
            {
                throw new GroupNotFoundException($"Group with id: {groupId} not found!");
            }

            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == groupId && x.IsMaster);
            return group?.ConnectionId;
        }

        public async Task<string> GetConnectionIdOfUser(string email, Guid groupId)
        {
            if (!(await this.GroupExistsAsync(groupId)))
            {
                return string.Empty;
            }

            var userId = this.dBContext.Users.FirstOrDefault(x => x.Email == email);

            return this.dBContext.Groups.FirstOrDefault(x => x.GroupName == groupId && x.UserId == userId.Id).ConnectionId;
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

        public async Task<long> GetIdOfMaster(Guid groupId)
        {
            if (!(await this.GroupExistsAsync(groupId)))
            {
                throw new GroupNotFoundException($"Group with id: {groupId} not found!");
            }

            var group = this.dBContext.Groups.FirstOrDefault(x => x.GroupName == groupId && x.IsMaster);
            return group.UserId;
        }



        public async Task<bool> IsUserAlreadyInGroupAsync(string userId, bool isMaster)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.UserId == long.Parse(userId) && x.IsMaster == isMaster);
            return group != null;
        }

        public async Task<string[]> GetEmailList(Guid groupId)
        {
            return this.dBContext.Groups.Where(x => x.GroupName == groupId).Select(x => x.Email).ToArray();

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

        public async Task<bool> IsUserAlreadyPartOfGroupWithOthers(string connectionId)
        {
            var group = this.dBContext.Groups.FirstOrDefault(x => x.ConnectionId == connectionId);
            var groups = this.dBContext.Groups.Where(x => x.GroupName == group.GroupName);
            return groups.Count() > 1;
        }
    }
}
