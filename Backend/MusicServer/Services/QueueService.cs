using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.Requests.Song;
using MusicServer.Entities.Requests.User;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using System.Collections.Generic;

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

        public async Task<PlaylistSongDto[]> CreateQueue(PlaylistSongDto[] songs, bool orderRandom, int playFromOrder)
        {
            var rnd = new Random();
            var userId = this.activeUserService.Id;
            await this.ClearQueue();

            if (playFromOrder > -1)
            {
                var indexFromWhichToPlay = songs.ToList().FindIndex(x => x.Order == playFromOrder);

                songs = songs.Skip(indexFromWhichToPlay).ToArray();
            }


            if (orderRandom)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }

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

        public async Task<PlaylistSongDto[]> CreateQueue(SongDto[] songs, bool orderRandom)
        {
            var rnd = new Random();
            var userId = this.activeUserService.Id;
            await this.ClearQueue();

            if (orderRandom)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }

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
                var mappedSong = this.mapper.Map<PlaylistSongDto>(songs[i]);
                mappedSong.Order = i;
                mappedSongs.Add(mappedSong);
            }

            await this.dbContext.SaveChangesAsync();

            // Return only the first 30
            return mappedSongs.OrderBy(x => x.Order).Take(30).ToArray(); ;
        }

        public async Task<PlaylistSongDto[]> GetCurrentQueue()
        {
            var userId = this.activeUserService.Id;
            // Only return the current and next songs in the queue
            var queue = this.dbContext.Queues
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId && x.Order > -1);

            if (queue.Count() == 0)
            {
                return new PlaylistSongDto[0];
            }

            var mappedSongs = this.mapper.Map<PlaylistSongDto[]>(
                queue.OrderBy(x => x.Order).Take(30).ToArray()
                );

            foreach (var song in mappedSongs)
            {
                // Check if song is in favorites
                if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userId && x.FavoriteSong.Id == song.Id) != null)
                {
                    song.IsInFavorites = true;
                }
            }

            return mappedSongs;
        }

        public async Task<PlaylistSongDto> GetCurrentSongInQueue()
        {
            var userId = this.activeUserService.Id;
            var song = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .FirstOrDefault(x => x.UserId == userId && x.Order == 0) ?? throw new SongNotFoundException();

            var mappedSong = this.mapper.Map<PlaylistSongDto>(song);

            // Check if song is in favorites
            if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userId && x.FavoriteSong.Id == mappedSong.Id) != null)
            {
                mappedSong.IsInFavorites = true;
            }

            return mappedSong;
        }

        public async Task<PlaylistSongDto> SkipForwardInQueue()
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId);

            // IF queue is empty
            if (queue.Count() == 0)
            {
                throw new SongNotFoundException();
            }

            // If there are no next songs
            if (!queue.Any(x => x.Order > 0))
            {
                throw new SongNotFoundException();
            }

            foreach (var queueEntity in queue)
            {
                queueEntity.Order = queueEntity.Order + (-1);
            }

            await this.dbContext.SaveChangesAsync();
            //TODO: If there is no next song call get current queue from frontend
            return await this.GetCurrentSongInQueue();
        }

        public async Task<PlaylistSongDto> SkipForwardInQueue(int index)
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId);

            var targetSong = this.dbContext.Queues.Where(x => x.UserId == userId && x.Order == index) ?? throw new SongNotFoundException();

            var subtractValue = 0 - index;

            foreach (var queueEntity in queue)
            {
                queueEntity.Order = queueEntity.Order + (subtractValue);
            }

            await this.dbContext.SaveChangesAsync();
            //TODO: If there is no next song call get current queue from frontend
            return await this.GetCurrentSongInQueue();
        }

        public async Task<PlaylistSongDto> SkipBackInQueue()
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId);

            if (!queue.Any(x => x.Order < 0))
            {
                // In frontend replay the current song
                throw new SongNotFoundException();
            }

            foreach (var queueEntity in queue)
            {
                queueEntity.Order = queueEntity.Order + 1;
            }

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueue();
        }

        public async Task<PlaylistSongDto[]> PushSongToIndex(int srcIndex, int targetIndex)
        {
            var userId = this.activeUserService.Id;
            var songToMove = this.dbContext.Queues.FirstOrDefault(x => x.Order == srcIndex && x.UserId == userId)
                ?? throw new SongNotFoundException();

            var targetPlace = this.dbContext.Queues.FirstOrDefault(x => x.Order == targetIndex && x.UserId == userId)
                ?? throw new SongNotFoundException();

            var oldSongOrder = songToMove.Order;
            songToMove.Order = targetIndex;

            var queueToTraverse = this.dbContext.Queues.Where(x => x.Order <= targetIndex && x.Id != songToMove.Id && x.Order > oldSongOrder && x.UserId == userId);

            if (oldSongOrder > targetIndex)
            {
                queueToTraverse = this.dbContext.Queues.Where(x => x.Order >= targetIndex && x.Id != songToMove.Id && x.Order < oldSongOrder && x.UserId == userId);
            }

            foreach (var songBefore in queueToTraverse)
            {
                if (oldSongOrder >= targetIndex)
                {
                    songBefore.Order++;
                    continue;
                }

                songBefore.Order--;
            }

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentQueue();
        }

        public async Task<PlaylistSongDto[]> RemoveSongsWithIndexFromQueue(int[] indices)
        {
            var userId = this.activeUserService.Id;
            
            // Remove target songs
            foreach (var index in indices)
            {
                var queueEntity = this.dbContext.Queues.FirstOrDefault(x => x.Order == index && x.UserId == userId) ?? throw new SongNotFoundException();

                this.dbContext.Queues.Remove(queueEntity);
            }

            // The only items that can be removed are next songs
            var queue = this.dbContext.Queues.Where(x => x.UserId == userId && x.Order > 0).OrderBy(x => x.Order);
            var newIndex = 1;
            // Fix the order of the songs
            foreach (var item in queue)
            {
                item.Order = newIndex;
                newIndex++;
            }

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentQueue();
        }

        public async Task<PlaylistSongDto[]> RandomizeQueue(PlaylistSongDto[] songs)
        {
            var userId = this.activeUserService.Id;
            var rnd = new Random();

            var currentSong = this.dbContext.Queues
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.UserId == userId) ?? throw new SongNotFoundException();

            await this.ClearQueue();

            var currentSongInSongs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).FirstOrDefault() ?? throw new SongNotFoundException();

            if (!songs.Any(x => x.Id == currentSong.Song.Id))
            {
                throw new SongNotFoundException();
            }

            // Push current playing song to top and remove it
            songs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).Skip(1).ToArray();

                // Order items random
            songs = songs.OrderBy(x => rnd.Next()).ToArray();

            // Add current playing song back to array
            songs = songs.Prepend(currentSongInSongs).ToArray();

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

        public async Task<PlaylistSongDto[]> RandomizeQueue(SongDto[] songs)
        {
            var userId = this.activeUserService.Id;
            var rnd = new Random();

            var currentSong = this.dbContext.Queues
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.UserId == userId) ?? throw new SongNotFoundException();

            await this.ClearQueue();

            if (!songs.Any(x => x.Id == currentSong.Song.Id ))
            {
                throw new SongNotFoundException();
            }

            var currentSongInSongs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).FirstOrDefault() ?? throw new SongNotFoundException();

            // Push current playing song to top and remove it
            songs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).Skip(1).ToArray();

            // Order items random
            songs = songs.OrderBy(x => rnd.Next()).ToArray();

            // Add current playing song back to array
            songs = songs.Prepend(currentSongInSongs).ToArray();

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
                var mappedSong = this.mapper.Map<PlaylistSongDto>(songs[i]);
                mappedSong.Order = i;
                mappedSongs.Add(mappedSong);
                mappedSongs[i].Order = i;
            }

            await this.dbContext.SaveChangesAsync();
            return mappedSongs.ToArray();
        }

        public async Task<PlaylistSongDto> GetSongInQueueWithIndex(int index)
        {
            if (index - 1 < 0)
            {
                throw new SongNotFoundException();
            }

            var userId = this.activeUserService.Id;
            var song = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .FirstOrDefault(x => x.UserId == userId && x.Order == (index - 1)) ?? throw new SongNotFoundException();

            var mappedSong = this.mapper.Map<PlaylistSongDto>(song);

            // Check if song is in favorites
            if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userId && x.FavoriteSong.Id == mappedSong.Id) != null)
            {
                mappedSong.IsInFavorites = true;
            }

            return mappedSong;
        }

        public async Task AddSongsToQueue(Guid[] songIds)
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId && x.Order > 0);

            foreach (var item in queue)
            {
                item.Order = item.Order + songIds.Length;
            }

            int order = 1;
            foreach (var songId in songIds)
            {
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = order,
                    Song = song,
                    UserId = userId
                });
                order = order + 1;
            }

            await this.dbContext.SaveChangesAsync();
        }
    }
}
