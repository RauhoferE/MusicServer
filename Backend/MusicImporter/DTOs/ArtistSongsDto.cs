using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.DTOs
{
    public class ArtistSongsDto
    {
        public Guid ArtistId { get; set; }

        public List<Guid> SongIds { get; set; }
    }
}
