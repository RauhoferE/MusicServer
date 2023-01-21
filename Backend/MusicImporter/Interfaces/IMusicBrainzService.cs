using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.Interfaces
{
    public interface IMusicBrainzService
    {
        public Task DownloadAlbumCover(string albumName, Guid albumId);

        public Task<DateTime> GetAlbumReleaseDate(string albumName);
    }
}
