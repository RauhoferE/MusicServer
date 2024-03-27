using AutoMapper;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicServer.Core.Const;
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

        public async Task ClearQueueAsync()
        {
            var userId = this.activeUserService.Id;

            var queue = this.dbContext.Queues.Where(x => (x.UserId == userId && !x.AddedManualy) || (x.UserId == userId && x.Order == 0));
            //var queuData = this.dbContext.QueueData.Where(x => x.UserId == userId);
            this.dbContext.Queues.RemoveRange(queue);
            //this.dbContext.QueueData.RemoveRange(queuData);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task ClearManuallyAddedQueueAsync()
        {
            var userId = this.activeUserService.Id;
            // Manually added songs disapear from the queue after being played or skiped
            var queue = this.dbContext.Queues.Where(x => x.UserId == userId && x.AddedManualy && x.Order != 0);
            //var queuData = this.dbContext.QueueData.Where(x => x.UserId == userId);
            this.dbContext.Queues.RemoveRange(queue);

            await this.dbContext.SaveChangesAsync();

            // Reorder rest of elements
            var otherqueue = this.dbContext.Queues.Where(x => x.UserId == userId && x.Order > 0).ToArray();

            for (int i = 0; i < otherqueue.Count(); i++)
            {
                var entity = otherqueue[i];
                entity.Order = i + 1;
            }

            //this.dbContext.QueueData.RemoveRange(queuData);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistSongDto> CreateQueueAsync(PlaylistSongDto[] songs, bool orderRandom, int playFromOrder)
        {
            var rnd = new Random();
            var userId = this.activeUserService.Id;
            await this.ClearQueueAsync();

            if (orderRandom && playFromOrder > -1)
            {
                var indexFromWhichToPlay = songs.ToList().FindIndex(x => x.Order == playFromOrder);
                var songToAppend = songs[indexFromWhichToPlay];

                songs = songs.Where(x => x.Id != songToAppend.Id).ToArray();
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
                songs = songs.Prepend(songToAppend).ToArray();
            }

            if (orderRandom && playFromOrder == -1)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }

            int subtractFromOrder = 0;

            if (playFromOrder != -1 && !orderRandom)
            {
                var indexFromWhichToPlay = songs.ToList().FindIndex(x => x.Order == playFromOrder);
                subtractFromOrder = 0 - indexFromWhichToPlay;
            }

            int lastStop = 0;

            for (int i = 0; i < songs.Length; i++)
            {
                if (subtractFromOrder + i > 0)
                {
                    lastStop = i;
                    break;
                }

                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = i + subtractFromOrder,
                    Song = song,
                    UserId = userId,
                    AddedManualy = false
                });
            }

            var lastManuallyAddedSong = this.dbContext.Queues.Where(x => x.UserId == userId && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 0 : lastManuallyAddedSong.Order;

            for (int i = lastStop; i < songs.Length; i++)
            {
                lastOrder++;
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    UserId = userId,
                    AddedManualy = false
                });
            }



            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<PlaylistSongDto> CreateQueueAsync(SongDto[] songs, bool orderRandom, int playFromOrder)
        {
            var rnd = new Random();
            var userId = this.activeUserService.Id;
            await this.ClearQueueAsync();

            if (orderRandom && playFromOrder > -1)
            {
                var songToAppend = songs[playFromOrder];

                songs = songs.Where(x => x.Id != songToAppend.Id).ToArray();
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
                songs = songs.Prepend(songToAppend).ToArray();
            }

            if (orderRandom && playFromOrder == -1)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }

            int subtractFromOrder = 0;

            if (playFromOrder != -1 && !orderRandom)
            {
                subtractFromOrder = 0 - playFromOrder;
            }

            int lastStop = 0;

            for (int i = 0; i < songs.Length; i++)
            {
                if (subtractFromOrder + i > 0)
                {
                    lastStop = i;
                    break;
                }

                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = i + subtractFromOrder,
                    Song = song,
                    UserId = userId
                });
            }

            var lastManuallyAddedSong = this.dbContext.Queues.Where(x => x.UserId == userId && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 0 : lastManuallyAddedSong.Order;

            for (int i = lastStop; i < songs.Length; i++)
            {
                lastOrder++;
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    UserId = userId,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<QueueSongDto[]> GetCurrentQueueAsync()
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
                return new QueueSongDto[0];
            }

            var mappedSongs = this.mapper.Map<QueueSongDto[]>(
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

        public async Task<PlaylistSongDto> GetCurrentSongInQueueAsync()
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

        public async Task<PlaylistSongDto> GetCurrentSongInQueueOfUserAsync(long userId)
        {
            var activeUserId = this.activeUserService.Id;
            var song = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .FirstOrDefault(x => x.UserId == userId && x.Order == 0) ?? throw new SongNotFoundException();

            var mappedSong = this.mapper.Map<PlaylistSongDto>(song);

            // Check if song is in favorites
            if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == activeUserId && x.FavoriteSong.Id == mappedSong.Id) != null)
            {
                mappedSong.IsInFavorites = true;
            }

            return mappedSong;
        }

        public async Task<PlaylistSongDto> SkipForwardInQueueAsync()
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

            QueueEntity toRemove = queue.FirstOrDefault(x => x.Order == 0);

            foreach (var queueEntity in queue)
            {
                if (toRemove.AddedManualy && queueEntity.Order <= 0)
                {
                    continue;
                }

                queueEntity.Order = queueEntity.Order + (-1);               
               
            }

            // Remove manually added queue item
            if (toRemove.AddedManualy)
            {
                this.dbContext.Queues.Remove(toRemove);
            }

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<PlaylistSongDto> SkipForwardInQueueAsync(int index)
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Order);

            var targetSong = this.dbContext.Queues.FirstOrDefault(x => x.UserId == userId && x.Order == index) ?? throw new SongNotFoundException();

            var subtractValue = 0 - index;

            List<QueueEntity> toRemove = new List<QueueEntity>();

            foreach (var queueEntity in queue)
            {
                if (queueEntity.Order + (subtractValue) < 0 && queueEntity.AddedManualy)
                {
                    // Remove song from list
                    toRemove.Add(queueEntity);
                    subtractValue = subtractValue + 1;
                    continue;
                }

                queueEntity.Order = queueEntity.Order + (subtractValue);
            }

            this.dbContext.RemoveRange(toRemove);

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<PlaylistSongDto> SkipBackInQueueAsync()
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

            var manuallyAddedSongCount = this.dbContext.Queues.Count(x => x.UserId == userId && x.AddedManualy &&  x.Order != 0);

            int addToOrder = 1;

            List<QueueEntity> toRemove = new List<QueueEntity>();

            foreach (var queueEntity in queue)
            {
                // If the current song was manually added remove it
                if (queueEntity.Order == 0 && queueEntity.AddedManualy)
                {
                    // Remove manually added and currently playing song
                    addToOrder--;
                    toRemove.Add(queueEntity);
                    continue;
                }

                // If the current song wasnt manually added put them at the end of the manually added songs
                if (queueEntity.Order == 0 && !queueEntity.AddedManualy && manuallyAddedSongCount > 0)
                {
                    queueEntity.Order = addToOrder + manuallyAddedSongCount;
                    continue;
                }

                // Let the manually added songs where they are
                if (queueEntity.Order > 0 && queueEntity.AddedManualy)
                {
                    continue;
                }

                queueEntity.Order = queueEntity.Order + addToOrder;
            }

            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<QueueSongDto[]> PushSongToIndexAsync(int srcIndex, int targetIndex, int markAsAddedManually)
        {
            var userId = this.activeUserService.Id;
            var songToMove = this.dbContext.Queues.FirstOrDefault(x => x.Order == srcIndex && x.UserId == userId)
                ?? throw new SongNotFoundException();

            var targetPlace = this.dbContext.Queues.FirstOrDefault(x => x.Order == targetIndex && x.UserId == userId)
                ?? throw new SongNotFoundException();

            var oldSongOrder = songToMove.Order;
            songToMove.Order = targetIndex;

            // If the song gets moved to the manually added ones mark it as manually added
            // If the song gets moved out of the manually added ones mark it as not manually added so it will disapear when you reshuffle the queue.
            songToMove.AddedManualy = targetPlace.AddedManualy;
            if (markAsAddedManually == 0)
            {
                songToMove.AddedManualy = false;
            }

            if (markAsAddedManually == 1)
            {
                songToMove.AddedManualy = true;
            }


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
            return await this.GetCurrentQueueAsync();
        }

        public async Task RemoveSongsWithIndexFromQueueAsync(int[] indices)
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
        }

        public async Task<PlaylistSongDto> ChangeQueueAsync(PlaylistSongDto[] songs, bool randomize)
        {
            var userId = this.activeUserService.Id;
            var rnd = new Random();

            var currentSong = this.dbContext.Queues
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.UserId == userId) ?? throw new SongNotFoundException();

            await this.ClearQueueAsync();

            bool isCurrentPlayingSongFromPlaylist = songs.Any(x => x.Id == currentSong.Song.Id && !currentSong.AddedManualy);

            if (isCurrentPlayingSongFromPlaylist)
            {
                // Push current playing song to top and remove it
                songs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).Skip(1).ToArray();
            }

            if (randomize)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }


            // Add First song back
            var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == currentSong.Song.Id) ?? throw new SongNotFoundException();
            this.dbContext.Queues.Add(new QueueEntity()
            {
                Order = 0,
                Song = song,
                UserId = userId,
                AddedManualy = false
            });


            var lastManuallyAddedSong = this.dbContext.Queues.Where(x => x.UserId == userId && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 1 : lastManuallyAddedSong.Order;

            for (int i = 0; i < songs.Length; i++)
            {
                lastOrder++;
                song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    UserId = userId,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<PlaylistSongDto> ChangeQueueAsync(SongDto[] songs, bool randomize)
        {
            var userId = this.activeUserService.Id;
            var rnd = new Random();

            var currentSong = this.dbContext.Queues
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.UserId == userId) ?? throw new SongNotFoundException();

            await this.ClearQueueAsync();

            bool isCurrentPlayingSongFromPlaylist = songs.Any(x => x.Id == currentSong.Song.Id && !currentSong.AddedManualy);

            if (isCurrentPlayingSongFromPlaylist)
            {
                // Push current playing song to top and remove it
                songs = songs.OrderByDescending(x => x.Id == currentSong.Song.Id).Skip(1).ToArray();
            }

            if (randomize)
            {
                // Order items random
                songs = songs.OrderBy(x => rnd.Next()).ToArray();
            }


            // Add First song back
            var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == currentSong.Song.Id) ?? throw new SongNotFoundException();
            this.dbContext.Queues.Add(new QueueEntity()
            {
                Order = 0,
                Song = song,
                UserId = userId,
                AddedManualy = false
            });

            var lastManuallyAddedSong = this.dbContext.Queues.Where(x => x.UserId == userId && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 1 : lastManuallyAddedSong.Order;

            for (int i = 0; i < songs.Length; i++)
            {
                lastOrder++;
                song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    UserId = userId,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync();
        }

        public async Task<PlaylistSongDto> GetSongInQueueWithIndexAsync(int index)
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

        public async Task AddSongsToQueueAsync(Guid[] songIds)
        {
            var userId = this.activeUserService.Id;
            var queue = this.dbContext.Queues
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.UserId == userId && x.Order > 0 && !x.AddedManualy);

            if (this.dbContext.QueueData.FirstOrDefault(x => x.UserId == userId) == null)
            {
                throw new DataNotFoundException();
            }

            foreach (var item in queue)
            {
                item.Order = item.Order + songIds.Length;
            }

            var manuallyAddedSongCount = this.dbContext.Queues.Count(x => x.UserId == userId && x.Order > 0 && x.AddedManualy);

            int order = 1 + manuallyAddedSongCount;
            foreach (var songId in songIds)
            {
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();
                this.dbContext.Queues.Add(new QueueEntity()
                {
                    Order = order,
                    Song = song,
                    UserId = userId,
                    AddedManualy = true
                });
                order = order + 1;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task UpdateQueueDataAsync(Guid itemId, string loopMode, string sortAfter, string target, bool randomize, bool asc)
        {
            var userId = this.activeUserService.Id;

            var queueData = this.dbContext.QueueData.Include(x => x.SortAfter)
                .Include(x => x.Target)
    .Include(x => x.LoopMode)
    .FirstOrDefault(x => x.UserId == userId);

            if (queueData == null)
            {
                queueData = new QueueData();
                queueData.UserId = userId;
                this.dbContext.QueueData.Add(queueData);
            }

            queueData.ItemId = itemId;
            queueData.Asc = asc;
            queueData.Random = randomize;

            switch (loopMode)
            {
                case LoopMode.None:
                    queueData.LoopMode = this.dbContext.LovLoopModes.First(x => x.Name == LoopMode.None);
                    break;
                case LoopMode.Audio:
                    queueData.LoopMode = this.dbContext.LovLoopModes.First(x => x.Name == LoopMode.Audio);
                    break;
                case LoopMode.Playlist:
                    queueData.LoopMode = this.dbContext.LovLoopModes.First(x => x.Name == LoopMode.Playlist);
                    break;
                default:
                    break;
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Artist:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.Artist);
                    break;
                case SortingElementsOwnPlaylistSongs.Order:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.Order);
                    break;
                case SortingElementsOwnPlaylistSongs.Duration:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.Duration);
                    break;
                case SortingElementsOwnPlaylistSongs.Album:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.Album);
                    break;
                case SortingElementsOwnPlaylistSongs.Name:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.Name);
                    break;
                case SortingElementsOwnPlaylistSongs.DateAdded:
                    queueData.SortAfter = this.dbContext.LovPlaylistSortAfter.First(x => x.Name == SortingElementsOwnPlaylistSongs.DateAdded);
                    break;
                default:
                    break;
            }

            switch (target)
            {
                case QueueTarget.Playlist:
                    queueData.Target = this.dbContext.LovQueueTargets.First(x => x.Name == QueueTarget.Playlist);
                    break;
                case QueueTarget.Song:
                    queueData.Target = this.dbContext.LovQueueTargets.First(x => x.Name == QueueTarget.Song);
                    break;
                case QueueTarget.Favorites:
                    queueData.Target = this.dbContext.LovQueueTargets.First(x => x.Name == QueueTarget.Favorites);
                    break;
                case QueueTarget.Album:
                    queueData.Target = this.dbContext.LovQueueTargets.First(x => x.Name == QueueTarget.Album);
                    break;
                default:
                    break;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<QueueDataDto> GetQueueDataAsync()
        {
            var userId = this.activeUserService.Id;
            var queueData = this.dbContext.QueueData.Include(x => x.SortAfter)
                .Include(x => x.Target)
                .Include(x => x.LoopMode)
                .FirstOrDefault(x => x.UserId == userId) ?? throw new DataNotFoundException("No queue Data available");

            return this.mapper.Map<QueueDataDto>(queueData);
        }

        public async Task<QueueEntity[]> GetAllQueueEntitiesAsync()
        {
            return this.dbContext.Queues
                .Include(x => x.Song)
                .Where(x => x.UserId ==  this.activeUserService.Id).ToArray();
        }

        public async Task<QueueData> GetQueueDataEntityAsync()
        {
            return this.dbContext.QueueData
                .Include(x => x.SortAfter)
                .Include(x => x.Target)
                .Include(x => x.LoopMode)
                .FirstOrDefault(x => x.UserId == this.activeUserService.Id);
        }


    }
}
