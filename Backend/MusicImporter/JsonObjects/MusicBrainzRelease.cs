using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusicImporter.JsonObjects
{
    public class MusicBrainzRelease
    {
        public string Id { get; set; }

        public DateTime Date { get; set; }

        [JsonPropertyName("release-group")]
        public ReleaseGroup ReleaseGroup {get; set;}

        [JsonPropertyName("artist-credit")]
        public ArtistCredit[] ArtistCredits { get; set; }
    }
}
