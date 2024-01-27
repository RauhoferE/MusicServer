using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicServer.Core.Const
{
    public static class SortingElementsOwnPlaylists
    {
        public const string Name = "name";
        public const string DateCreated = "created";
        public const string NumberOfSongs = "songCount";
        public const string Custom = "order";


        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, DateCreated, NumberOfSongs, Custom
        };
    }

    public static class SortingElementsOwnPlaylistSongs
    {
        public const string Name = "name";
        public const string Duration = "duration";
        public const string DateAdded = "created";
        public const string Order = "order";
        public const string Artist = "artist";
        public const string Album = "album";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, Duration, DateAdded, Order, Artist, Album
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

    public static class SortingElementsAllArtistsAndUsers
    {
        public const string Name = "Name";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name
        };
    }

    public static class SortingElementsAllAlbums
    {
        public const string Name = "Name";
        public const string NumberOfSongs = "NumberOfSongs";
        public const string Artist = "Artist";
        public const string DateAdded = "DateAdded";

        // Return all constants in a list
        public static List<string> SortingElements = new List<string>()
        {
            Name, NumberOfSongs, Artist, DateAdded
        };
    }
}
