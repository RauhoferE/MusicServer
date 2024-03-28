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
    public class GroupQueueService : IGroupQueueService
    {
        private readonly MusicServerDBContext dbContext;

        private readonly IMapper mapper;

        public GroupQueueService(MusicServerDBContext dBContext, IMapper mapper)
        {
            this.dbContext = dBContext;
            this.mapper = mapper;
        }

        public async Task AddSongsToQueueAsync(Guid groupName, Guid[] songIds)
        {
            var queue = this.dbContext.GroupQueueEntities
    .Include(x => x.Song)
    .ThenInclude(x => x.Artists)
    .ThenInclude(x => x.Artist)
    .Include(x => x.Song.Album)
    .Where(x => x.GroupId == groupName && x.Order > 0 && !x.AddedManualy);

            if (this.dbContext.GroupQueueData.FirstOrDefault(x => x.GroupId == groupName) == null)
            {
                throw new DataNotFoundException();
            }

            foreach (var item in queue)
            {
                item.Order = item.Order + songIds.Length;
            }

            var manuallyAddedSongCount = this.dbContext.GroupQueueEntities.Count(x => x.GroupId == groupName 
            && x.Order > 0 && x.AddedManualy);

            int order = 1 + manuallyAddedSongCount;
            foreach (var songId in songIds)
            {
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songId) ?? throw new SongNotFoundException();
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = order,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = true
                });
                order = order + 1;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistSongDto> ChangeQueueAsync(Guid groupName, PlaylistSongDto[] songs, bool randomize)
        {
            var rnd = new Random();

            var currentSong = this.dbContext.GroupQueueEntities
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.GroupId == groupName) ?? throw new SongNotFoundException();

            await this.ClearQueueAsync(groupName);

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
            this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
            {
                Order = 0,
                Song = song,
                GroupId = groupName,
                AddedManualy = false
            });


            var lastManuallyAddedSong = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 1 : lastManuallyAddedSong.Order;

            for (int i = 0; i < songs.Length; i++)
            {
                lastOrder++;
                song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task<PlaylistSongDto> ChangeQueueAsync(Guid groupName, SongDto[] songs, bool randomize)
        {
            var rnd = new Random();

            var currentSong = this.dbContext.GroupQueueEntities
                .Include(x => x.Song)
                .FirstOrDefault(x => x.Order == 0 && x.GroupId == groupName) ?? throw new SongNotFoundException();

            await this.ClearQueueAsync(groupName);

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
            this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
            {
                Order = 0,
                Song = song,
                GroupId = groupName,
                AddedManualy = false
            });

            var lastManuallyAddedSong = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 1 : lastManuallyAddedSong.Order;

            for (int i = 0; i < songs.Length; i++)
            {
                lastOrder++;
                song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task ClearManuallyAddedQueueAsync(Guid groupName)
        {
            // Manually added songs disapear from the queue after being played or skiped
            var queue = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.AddedManualy && x.Order != 0);
            //var queuData = this.dbContext.QueueData.Where(x => x.UserId == userId);
            this.dbContext.GroupQueueEntities.RemoveRange(queue);

            await this.dbContext.SaveChangesAsync();

            // Reorder rest of elements
            var otherqueue = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.Order > 0).ToArray();

            for (int i = 0; i < otherqueue.Count(); i++)
            {
                var entity = otherqueue[i];
                entity.Order = i + 1;
            }

            //this.dbContext.QueueData.RemoveRange(queuData);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task ClearQueueAsync(Guid groupName)
        {
            var queue = this.dbContext.GroupQueueEntities.Where(x => (x.GroupId == groupName && !x.AddedManualy) || (x.GroupId == groupName && x.Order == 0));
            //var queuData = this.dbContext.QueueData.Where(x => x.UserId == userId);
            this.dbContext.GroupQueueEntities.RemoveRange(queue);
            //this.dbContext.QueueData.RemoveRange(queuData);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistSongDto> CreateQueueAsync(Guid groupName, SongDto[] songs, bool orderRandom, int playFromOrder)
        {
            var rnd = new Random();
            await this.ClearQueueAsync(groupName);

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
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = i + subtractFromOrder,
                    Song = song,
                    GroupId = groupName
                });
            }

            var lastManuallyAddedSong = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 0 : lastManuallyAddedSong.Order;

            for (int i = lastStop; i < songs.Length; i++)
            {
                lastOrder++;
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = false
                });
            }

            await this.dbContext.SaveChangesAsync();

            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task<PlaylistSongDto> CreateQueueAsync(Guid groupName, PlaylistSongDto[] songs, bool orderRandom, int playFromOrder)
        {
            var rnd = new Random();
            await this.ClearQueueAsync(groupName);

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
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = i + subtractFromOrder,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = false
                });
            }

            var lastManuallyAddedSong = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.AddedManualy).OrderByDescending(x => x.Order).FirstOrDefault();

            var lastOrder = lastManuallyAddedSong == null ? 0 : lastManuallyAddedSong.Order;

            for (int i = lastStop; i < songs.Length; i++)
            {
                lastOrder++;
                var song = this.dbContext.Songs.FirstOrDefault(x => x.Id == songs[i].Id) ?? throw new SongNotFoundException();
                this.dbContext.GroupQueueEntities.Add(new GroupQueueEntity()
                {
                    Order = lastOrder,
                    Song = song,
                    GroupId = groupName,
                    AddedManualy = false
                });
            }



            await this.dbContext.SaveChangesAsync();
            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task<QueueSongDto[]> GetCurrentQueueAsync(Guid groupName)
        {
            // Only return the current and next songs in the queue
            var queue = this.dbContext.GroupQueueEntities
                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .Where(x => x.GroupId == groupName && x.Order > -1);

            if (queue.Count() == 0)
            {
                return new QueueSongDto[0];
            }

            var mappedSongs = this.mapper.Map<QueueSongDto[]>(
                queue.OrderBy(x => x.Order).Take(30).ToArray()
                );

            //foreach (var song in mappedSongs)
            //{
            //    // Check if song is in favorites
            //    if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userId && x.FavoriteSong.Id == song.Id) != null)
            //    {
            //        song.IsInFavorites = true;
            //    }
            //}

            //TODO: Mark returned songs with markqueuesongs as favorite
            return mappedSongs;
        }

        public async Task<QueueSongDto[]> MarkQueueSongsAsFavorite(long userID, QueueSongDto[] songs)
        {
            foreach (var song in songs)
            {
                // Check if song is in favorites
                if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userID && x.FavoriteSong.Id == song.Id) != null)
                {
                    song.IsInFavorites = true;
                }
            }

            return songs;
        }

        public async Task<PlaylistSongDto> MarkPlaylistSongAsFavorite(long userID, PlaylistSongDto song)
        {
            // Check if song is in favorites
            if (this.dbContext.FavoriteSongs.Include(x => x.User).Include(x => x.FavoriteSong).FirstOrDefault(x => x.User.Id == userID && x.FavoriteSong.Id == song.Id) != null)
            {
                song.IsInFavorites = true;
            }

            return song;
        }

        public async Task<PlaylistSongDto> GetCurrentSongInQueueAsync(Guid groupName)
        {
            var song = this.dbContext.GroupQueueEntities
                    .Include(x => x.Song)
    .ThenInclude(x => x.Artists)
    .ThenInclude(x => x.Artist)
    .Include(x => x.Song.Album)
    .FirstOrDefault(x => x.GroupId == groupName && x.Order == 0) ?? throw new SongNotFoundException();

            // TODO: Mark song as favorite in other method
            return this.mapper.Map<PlaylistSongDto>(song);
        }

        public async Task<QueueDataDto> GetQueueDataAsync(Guid groupName)
        {
            var queueData = this.dbContext.GroupQueueData.Include(x => x.SortAfter)
    .Include(x => x.Target)
    .Include(x => x.LoopMode)
    .FirstOrDefault(x => x.GroupId == groupName) ?? throw new DataNotFoundException("No queue Data available");

            var data = this.mapper.Map<QueueDataDto>(queueData);
            data.UserId = queueData.UserId;
            return data;
        }

        public async Task<PlaylistSongDto> GetSongInQueueWithIndexAsync(Guid groupName, int index)
        {
            if (index - 1 < 0)
            {
                throw new SongNotFoundException();
            }

            var song = this.dbContext.GroupQueueEntities
                                .Include(x => x.Song)
                .ThenInclude(x => x.Artists)
                .ThenInclude(x => x.Artist)
                .Include(x => x.Song.Album)
                .FirstOrDefault(x => x.GroupId == groupName && x.Order == (index - 1)) ?? throw new SongNotFoundException();

            var mappedSong = this.mapper.Map<PlaylistSongDto>(song);

            // TODO: Mark songs as favorite
            return mappedSong;
        }

        public async Task<QueueSongDto[]> PushSongToIndexAsync(Guid groupName, int srcIndex, int targetIndex, int markAsAddedManually)
        {
            var songToMove = this.dbContext.GroupQueueEntities.FirstOrDefault(x => x.Order == srcIndex 
            && x.GroupId == groupName)
                ?? throw new SongNotFoundException();

            var targetPlace = this.dbContext.GroupQueueEntities.FirstOrDefault(x => x.Order == targetIndex 
            && x.GroupId == groupName)
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


            var queueToTraverse = this.dbContext.GroupQueueEntities.Where(x => x.Order <= targetIndex && x.Id != songToMove.Id && x.Order > oldSongOrder && x.GroupId == groupName);

            if (oldSongOrder > targetIndex)
            {
                queueToTraverse = this.dbContext.GroupQueueEntities.Where(x => x.Order >= targetIndex && x.Id != songToMove.Id && x.Order < oldSongOrder && x.GroupId == groupName);
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
            //TODO: Mark songs as favorite
            return await this.GetCurrentQueueAsync(groupName);
        }

        public async Task RemoveSongsWithIndexFromQueueAsync(Guid groupName, int[] indices)
        {
            // Remove target songs
            foreach (var index in indices)
            {
                var queueEntity = this.dbContext.GroupQueueEntities.FirstOrDefault(x => x.Order == index 
                && x.GroupId == groupName) ?? throw new SongNotFoundException();

                this.dbContext.GroupQueueEntities.Remove(queueEntity);
            }

            // The only items that can be removed are next songs
            var queue = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName && x.Order > 0).OrderBy(x => x.Order);
            var newIndex = 1;
            // Fix the order of the songs
            foreach (var item in queue)
            {
                item.Order = newIndex;
                newIndex++;
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistSongDto> SkipBackInQueueAsync(Guid groupName)
        {
            var queue = this.dbContext.GroupQueueEntities
                     .Include(x => x.Song)
     .ThenInclude(x => x.Artists)
     .ThenInclude(x => x.Artist)
     .Include(x => x.Song.Album)
     .Where(x => x.GroupId == groupName);

            if (!queue.Any(x => x.Order < 0))
            {
                // In frontend replay the current song
                throw new SongNotFoundException();
            }

            var manuallyAddedSongCount = this.dbContext.GroupQueueEntities.Count(x => x.GroupId == groupName && x.AddedManualy && x.Order != 0);

            int addToOrder = 1;

            List<GroupQueueEntity> toRemove = new List<GroupQueueEntity>();

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
            // TODO: Mark songs as favorite
            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task<PlaylistSongDto> SkipForwardInQueueAsync(Guid groupName)
        {
            var queue = this.dbContext.GroupQueueEntities
                     .Include(x => x.Song)
     .ThenInclude(x => x.Artists)
     .ThenInclude(x => x.Artist)
     .Include(x => x.Song.Album)
     .Where(x => x.GroupId == groupName);

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

            GroupQueueEntity toRemove = queue.FirstOrDefault(x => x.Order == 0);

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
                this.dbContext.GroupQueueEntities.Remove(toRemove);
            }

            await this.dbContext.SaveChangesAsync();
            //TODO: Mark song as favorite
            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task<PlaylistSongDto> SkipForwardInQueueAsync(Guid groupName, int index)
        {
            var queue = this.dbContext.GroupQueueEntities
                    .Include(x => x.Song)
    .ThenInclude(x => x.Artists)
    .ThenInclude(x => x.Artist)
    .Include(x => x.Song.Album)
    .Where(x => x.GroupId == groupName);

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

            GroupQueueEntity toRemove = queue.FirstOrDefault(x => x.Order == 0);

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
                this.dbContext.GroupQueueEntities.Remove(toRemove);
            }

            await this.dbContext.SaveChangesAsync();
            // TODO: Mark song as favorite
            return await this.GetCurrentSongInQueueAsync(groupName);
        }

        public async Task UpdateQueueDataAsync(Guid groupName, Guid itemId, string loopMode, string sortAfter, string target, bool randomize, bool asc, long userId)
        {
            var queueData = this.dbContext.GroupQueueData.Include(x => x.SortAfter)
    .Include(x => x.Target)
.Include(x => x.LoopMode)
.FirstOrDefault(x => x.GroupId == groupName);

            if (queueData == null)
            {
                queueData = new GroupQueueData();
                queueData.GroupId = groupName;
                this.dbContext.GroupQueueData.Add(queueData);
            }

            queueData.ItemId = itemId;
            queueData.Asc = asc;
            queueData.Random = randomize;
            queueData.UserId = userId;

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

        public async Task SetQueueDataAsync(QueueData queueData)
        {
            var data = this.mapper.Map<GroupQueueData>(queueData);
            this.dbContext.GroupQueueData.Add(data);
        }

        public async Task SetQueueEntitiesAsync(QueueEntity[] queueEntities)
        {
            var entities = this.mapper.Map<GroupQueueEntity[]>(queueEntities);
            this.dbContext.GroupQueueEntities.AddRange(entities);
        }

        public async Task RemoveQueueDataAndEntitiesAsync(Guid groupName)
        {
            var groupData = this.dbContext.GroupQueueData.FirstOrDefault(x => x.GroupId == groupName);
            var groupEntities = this.dbContext.GroupQueueEntities.Where(x => x.GroupId == groupName);

            if (groupData != null)
            {
                this.dbContext.GroupQueueData.Remove(groupData);
            }

            this.dbContext.GroupQueueEntities.RemoveRange(groupEntities);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
