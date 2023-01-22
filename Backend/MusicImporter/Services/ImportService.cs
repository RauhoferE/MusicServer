using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using MusicImporter.DTOs;
using MusicImporter.Interfaces;
using MusicImporter.Settings;
using MusicServer.Core.Interfaces;
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

        public ImportService(FileserverSettings fileserverSettings, 
            MusicDataSettings musicDataSettings,
            MusicServerDBContext dBContext,
            ISftpService sftpService,
            IMusicBrainzService musicBrainzService)
        {
            this.fileserverSettings = fileserverSettings;
            this.musicDataSettings= musicDataSettings;
            this.dBContext = dBContext;
            this.sftpService = sftpService;
            this.musicBrainzService = musicBrainzService;
        }

        public Task CopyMp3ToFileServer(string file, Guid songId)
        {
            throw new NotImplementedException();
        }

        public async Task<ID3MetaData> GetMetaDataFromMp3(string file)
        {
            var mp3 = TagLib.File.Create(file);
            // Get length of mp3 from ffmpeg
            return new ID3MetaData()
            {
                Album = mp3.Tag.Album,
                AlbumArtists = mp3.Tag.AlbumArtists,
                SongArtists = mp3.Tag.Performers,
                Name = mp3.Name
            };
        }

        public async Task<Guid> ImportMp3DataToDatabase(ID3MetaData metaData)
        {
            // Check if artists exist
            // Check if album exists
            // Add new Song



            var  album = this.dBContext.Albums.FirstOrDefault(x => x.Name == metaData.Album);

            if (album == null)
            {
                album = this.dBContext.Albums.Add(new Album()
                {
                    Name = metaData.Album,
                    Created = metaData.ReleaseDate
                }).Entity;
            }

            var song = this.dBContext.Songs.Add(new Song()
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
                }

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
            return song.Id;
        }

        public async Task StartImportProcess()
        {
            if (!Directory.Exists(this.musicDataSettings.SourceFolder))
            {
                throw new DirectoryNotFoundException("Music Sourcefolder was not found");
            }

            foreach (var song in Directory.GetFiles(this.musicDataSettings.SourceFolder, "*.mp3"))
            {
                var data = await this.GetMetaDataFromMp3(song);
            }
        }
    }
}
