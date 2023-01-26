using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicImporter.DTOs
{
    public class ID3MetaData
    {
        public string Name { get; set; }

        public string Album { get; set; }

        public string[] SongArtists { get; set; }

        public string[] AlbumArtists { get; set; }

        public double Length { get; set; }
    }
}
