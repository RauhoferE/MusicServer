using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public static class SortingElementsAlbumSongs
    {
        public const string Name = "Name";
        public const string Duration = "Duration";
        public const string DateAdded = "DateAdded";
        public const string Artist = "Artist";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, Duration, DateAdded, Artist
        };
    }

    public static class SortingElementsPlaylists
    {
        public const string Name = "Name";
        public const string DateCreated = "DateCreated";
        public const string Creator = "Creator";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, DateCreated, Creator
        };
    }

    public static class SortingElementsPlaylistSongs
    {
        public const string Name = "Name";
        public const string Duration = "Duration";
        public const string DateAdded = "DateAdded";
        public const string Custom = "Custom";
        public const string Artist = "Artist";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, Duration, DateAdded, Custom, Artist
        };
    }
}
