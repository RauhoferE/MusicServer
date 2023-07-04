using MusicImporter.DTOs;
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

        public Task<ID3MetaData> GetMetaDataFromMp3(string file);

        public Task<SongArtistsDto> ImportMp3DataToDatabase(ID3MetaData metaData);

        public Task CopyMp3ToFileServer(string file, Guid songId);
    }
}
