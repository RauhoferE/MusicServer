using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Interfaces
{
    public interface IImportService
    {
        public Task StartImportProcess();

        public Task GetMetaDataFromMp3(string file);

        public Task ImportMp3DataToDatabase();

        public Task CopyMp3ToFileServer(string file);
    }
}
