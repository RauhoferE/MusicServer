using DataAccess;
using MusicImporter.Interfaces;
using MusicImporter.Settings;
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

        public ImportService(FileserverSettings fileserverSettings, 
            MusicDataSettings musicDataSettings,
            MusicServerDBContext dBContext)
        {
            this.fileserverSettings = fileserverSettings;
            this.musicDataSettings= musicDataSettings;
            this.dBContext = dBContext;
        }

        public Task CopyMp3ToFileServer(string file)
        {
            throw new NotImplementedException();
        }

        public Task GetMetaDataFromMp3(string file)
        {
            throw new NotImplementedException();
        }

        public Task ImportMp3DataToDatabase()
        {
            throw new NotImplementedException();
        }

        public async Task StartImportProcess()
        {
            if (!Directory.Exists(this.musicDataSettings.SourceFolder))
            {
                throw new DirectoryNotFoundException("Music Sourcefolder was not found");
            }

            foreach (var folder in Directory.GetDirectories(this.musicDataSettings.SourceFolder))
            {
                foreach (var song in Directory.GetFiles(Path.GetFullPath(folder), "*.mp3"))
                {

                }
            }
        }
    }
}
