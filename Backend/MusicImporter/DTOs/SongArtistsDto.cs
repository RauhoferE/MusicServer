using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.DTOs
{
    public class SongArtistsDto
    {
        public Guid SongId { get; set; }

        public List<Guid> ArtistIds { get; set; }
    }
}
