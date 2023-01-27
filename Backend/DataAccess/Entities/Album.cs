using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Album : BaseEntity
    {
        public Guid Id { get; set; }

        public DateTime Release { get; set; }

        public string? Description { get; set; }

        public bool IsSingle { get; set; } = false;

        public ICollection<Song> Songs { get; set; } = new List<Song>();

        public ICollection<ArtistAlbum> Artists { get; set; } = new List<ArtistAlbum>();
    }
}
