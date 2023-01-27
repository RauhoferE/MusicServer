using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Artist : BaseEntity
    {
        public Guid Id { get; set; }

        public string? Description { get; set; }

        public ICollection<ArtistAlbum> Albums { get; set; } = new List<ArtistAlbum>();

        public ICollection<ArtistSong> Songs { get; set; } = new List<ArtistSong>();
    }
}
