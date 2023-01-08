using MusicImporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Services
{
    public class ImportService : IImportService
    {
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

        public Task StartImportProcess()
        {
            throw new NotImplementedException();
        }
    }
}
