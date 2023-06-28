using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public static class SortingElementsFavorites
    {
        public const string Name = "Name";
        public const string Duration = "Duration";
        public const string DateAdded = "DateAdded";
        public const string Artist = "Artist";
        private const string Custom = "Custom";


        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, Duration, DateAdded, Artist, Custom
        };
    }

    public static class SortingElementsOwnPlaylists
    {
        public const string Name = "Name";
        public const string DateCreated = "DateCreated";
        public const string Creator = "Creator";
        public const string NumberOfSongs = "NumberOfSongs";
        private const string Custom = "Custom";


        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, DateCreated, Creator, NumberOfSongs, Custom
        };
    }

    public static class SortingElementsOwnPlaylistSongs
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

    public static class SortingElementsAllSongs
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

    public static class SortingElementsPublicPlaylists
    {
        public const string Name = "Name";
        public const string DateCreated = "DateCreated";
        public const string NumberOfSongs = "NumberOfSongs";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, DateCreated, NumberOfSongs
        };
    }

    public static class SortingElementsAllArtists
    {
        public const string Name = "Name";
        public const string NumberOfSongs = "NumberOfSongs";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, NumberOfSongs
        };
    }

    public static class SortingElementsAllAlbums
    {
        public const string Name = "Name";
        public const string NumberOfSongs = "NumberOfSongs";
        public const string Artist = "Artist";
        private const string DateAdded = "DateAdded";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, NumberOfSongs, Artist, DateAdded
        };
    }
}
