using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public static class QueueTarget
    {
        public const string Favorites = "favorites";
        public const string Album = "album";
        public const string Playlist = "playlist";
        public const string Song = "song";

        public static List<string> QueueTargets = new List<string>()
        {
            Favorites, Album, Playlist, Song
        };
    }
}
