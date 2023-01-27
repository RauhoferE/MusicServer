﻿namespace MusicServer.Const
{
    public static class ApiRoutes
    {
        private const string Root = "api";

        private const string Controller = "[controller]";

        public const string Base = Root + "/" + Controller;

        public static class User
        {
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Register = "register";
            public const string ConfirmMail = "confirm/email/{email}/{token}";
        }

        public static class Development
        {
            public const string Test = "test";
            public const string CreateArtistsAndSongs = "create/{artists}/{albums}/{songs}";
        }

        public static class Playlist
        {
            public const string Default = "";
            public const string Songs = "songs";
            public const string Playlists = "playlists";
            public const string UserPlaylists = "user/{userId}";
            public const string PublicPlaylist = "public";
            public const string PlaylistSongs = "songs";
            public const string PlaylistAlbum = "album";
            public const string PlaylistShare = "share";
            public const string PlaylistCopy = "copy";
        }
    }
}
