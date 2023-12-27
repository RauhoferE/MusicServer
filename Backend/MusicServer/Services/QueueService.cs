using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Exceptions;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class QueueService : IQueueService
    {
        private readonly MusicServerDBContext dbContext;
        private readonly IActiveUserService activeUserService;
        private readonly IMapper mapper;

        public QueueService(MusicServerDBContext dBContext, IActiveUserService activeUserService, IMapper mapper)
        {
                this.dbContext = dBContext;
            this.activeUserService = activeUserService;
            this.mapper = mapper;
        }

        public async Task ClearQueue()
        {
            var userId = this.activeUserService.Id;

            var queue = this.dbContext.Queues.Where(x => x.UserId == userId);
            this.dbContext.Queues.RemoveRange(queue);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistSongDto[]> CreateQueue(PlaylistSongDto[] songs)
        {
            var userId = this.activeUserService.Id;
            await this.ClearQueue();

            for (int i = 0; i < songs.Length; i++)
            {
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = i,
                    Song = song,
                    UserId = userId
                });
                songs[i].Order = i;
            }

            await this.dbContext.SaveChangesAsync();
            return songs;
        }

        public async Task<PlaylistSongDto[]> CreateQueue(SongDto[] songs)
        {
            var userId = this.activeUserService.Id;
            await this.ClearQueue();
            List<PlaylistSongDto> mappedSongs = new List<PlaylistSongDto>();

            for (int i = 0; i < songs.Length; i++)
            {
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = i,
                    Song = song,
                    UserId = userId
                });
                var mappedSong = this.mapper.Map<PlaylistSongDto>(song);
                mappedSong.Order = i;
                mappedSongs.Add(mappedSong);
                
            }

            await this.dbContext.SaveChangesAsync();
            return mappedSongs.ToArray(); ;
        }

        public async Task<PlaylistSongDto[]> GetCurrentQueue()
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues.Where(x => x.UserId == userId);

            if (queue.Count() == 0)
            {
                return new PlaylistSongDto[0];
            }

            return this.mapper.Map<PlaylistSongDto[]>(queue.Select(x => x.Song).ToArray());
        }

        public Task<PlaylistSongDto> GetNextSongInQueue()
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSongDto> GetPreviousSongInQueue()
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSongDto[]> PushSongToIndex(int srcIndex, int targetIndex)
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSongDto[]> RandomizeQueue()
        {
            throw new NotImplementedException();
        }

        public Task<PlaylistSongDto[]> RemoveSongWithIndexFromQueue(int index)
        {
            throw new NotImplementedException();
        }
    }
}
