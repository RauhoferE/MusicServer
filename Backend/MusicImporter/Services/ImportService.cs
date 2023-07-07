using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicImporter.DTOs;
using MusicImporter.Exceptions;
using MusicImporter.Interfaces;
using MusicImporter.Settings;
using MusicServer.Core.Interfaces;
using MusicServer.Core.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Services
{
    public class ImportService : IImportService
    {
        private readonly FileserverSettings fileserverSettings;

        private readonly MusicDataSettings musicDataSettings;

        private readonly MusicServerDBContext dBContext;

        private readonly ISftpService sftpService;

        private readonly IMusicBrainzService musicBrainzService;

        private readonly IFfmpegService ffmpegService;

        private readonly IMessageService messageService;

        public ImportService(FileserverSettings fileserverSettings, 
            MusicDataSettings musicDataSettings,
            MusicServerDBContext dBContext,
            ISftpService sftpService,
            IMusicBrainzService musicBrainzService,
            IFfmpegService ffmpegService,
            IMessageService messageService)
        {
            this.fileserverSettings = fileserverSettings;
            this.musicDataSettings = musicDataSettings;
            this.dBContext = dBContext;
            this.sftpService = sftpService;
            this.musicBrainzService = musicBrainzService;
            this.ffmpegService = ffmpegService;
            this.messageService = messageService;
        }

        public async Task CopyMp3ToFileServer(string file, Guid songId)
        {
            if (!File.Exists(file))
            {
                throw new NotFoundException($"Song file {file} not found.");
            }
            
            using (var fs = new FileStream(file, FileMode.Open))
            {
                await this.sftpService.UploadFile(fs, $@"{fileserverSettings.SongFolder}/{songId}.mp3");
            }
        }

        public async Task<ID3MetaData> GetMetaDataFromMp3(string file)
        {
            var mp3 = TagLib.File.Create(file);

            var duration = await this.ffmpegService.GetDurationOfMp3(file);
            
            return new ID3MetaData()
            {
                Album = mp3.Tag.Album,
                AlbumArtists = mp3.Tag.AlbumArtists,
                SongArtists = mp3.Tag.Performers,
                Name = mp3.Tag.Title,
                Length = duration
            };
        }

        public async Task<SongArtistsDto> ImportMp3DataToDatabase(ID3MetaData metaData)
        {
            var  album = this.dBContext.Albums.FirstOrDefault(x => x.Name == metaData.Album);

            if (album == null)
            {
                var releaseDatae = await this.musicBrainzService.GetAlbumReleaseDate(metaData.Album);
                album = this.dBContext.Albums.Add(new Album()
                {
                    Name = metaData.Album,
                    Release = releaseDatae
                }).Entity;

                // Download Album Cover and Save it 
                await this.musicBrainzService.DownloadAlbumCover(metaData.Album, album.Id);
            }

            var song = this.dBContext.Songs.FirstOrDefault(x => x.Name == metaData.Name && x.Length == metaData.Length && x.Album == album);

            if (song != null)
            {
                throw new SongExistsException(song.Id.ToString());
            }

            song = this.dBContext.Songs.Add(new Song()
            {
                Length = metaData.Length,
                Name = metaData.Name,
                Album = album
            }).Entity;

            foreach (var artist in metaData.AlbumArtists)
            {
                var artistEntity = this.dBContext.Artists
                    .Include(x => x.Albums)
                    .ThenInclude(x => x.Album).FirstOrDefault(x => x.Name == artist);

                if (artistEntity == null)
                {
                    artistEntity = this.dBContext.Artists.Add(new Artist()
                    {
                        Name = artist
                    }).Entity;

                    await this.messageService.ArtistAddedMessage(artistEntity.Id);
                }

                var albumEntity = artistEntity.Albums.FirstOrDefault(x => x.Album.Name == album.Name);

                if (albumEntity == null)
                {
                    artistEntity.Albums.Add(new ArtistAlbum()
                    {
                        Album = album
                    });
                }

                await this.dBContext.SaveChangesAsync();
            }

            List<Guid> artistIds = new List<Guid>();

            foreach (var artist in metaData.SongArtists)
            {
                var artistEntity = this.dBContext.Artists
    .Include(x => x.Songs)
    .ThenInclude(x => x.Song).FirstOrDefault(x => x.Name == artist);

                if (artistEntity == null)
                {
                    artistEntity = this.dBContext.Artists.Add(new Artist()
                    {
                        Name = artist
                    }).Entity;

                    await this.messageService.ArtistAddedMessage(artistEntity.Id);
                }

                artistIds.Add(artistEntity.Id);

                var songEntity = artistEntity.Songs.FirstOrDefault(x => x.Song.Name == song.Name);

                if (songEntity == null)
                {
                    artistEntity.Songs.Add(new ArtistSong()
                    {
                        Song = song
                    });
                }

                await this.dBContext.SaveChangesAsync();
            }

            await this.dBContext.SaveChangesAsync();
            return new SongArtistsDto()
            {
                SongId = song.Id,
                ArtistIds = artistIds
            };
        }

        public async Task StartImportProcess()
        {
            if (!Directory.Exists(this.musicDataSettings.SourceFolder))
            {
                throw new DirectoryNotFoundException("Music Sourcefolder was not found");
            }

            foreach (var song in Directory.GetFiles(this.musicDataSettings.SourceFolder, "*.mp3"))
            {
                Log.Information($"Starting to import file: {song}");

                try
                {
                    var data = await this.GetMetaDataFromMp3(song);
                    var importResult = await this.ImportMp3DataToDatabase(data);

                    foreach (var artist in importResult.ArtistIds)
                    {
                        await this.messageService.ArtistSongsAddedMessage(artist, new List<Guid>() { importResult.SongId });
                    }

                    await this.CopyMp3ToFileServer(song, importResult.SongId);
                }
                catch (SongExistsException ex)
                {
                    Log.Information($"Song already imported: {ex.Message}");
                }
                catch(NotFoundException ex)
                {
                    Log.Information($"{ex.Message}");
                }
            }
        }
    }
}
