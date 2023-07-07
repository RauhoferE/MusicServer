using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MusicImporter.JsonObjects
{
    public class ReleaseGroup
    {
        [JsonPropertyName("primary-type")]
        public string PrimaryType { get; set; }
    }
}
