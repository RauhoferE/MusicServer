using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public static class LoopMode
    {
        public const string None = "none";
        public const string Audio = "audio";
        public const string Playlist = "playlist";

        public static List<string> LoopModes = new List<string>()
        {
            None, Audio, Playlist
        };
    }
}
