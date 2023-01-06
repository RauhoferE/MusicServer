namespace MusicServer.Const
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

        public static class Song
        {
            public const string Playlist = "playlist";
            public const string PlaylistSongs = "playlist/songs";
            public const string PlaylistAlbum = "playlist/album";
            public const string PlaylistShare = "playlist/share";
            public const string PlaylistCopy = "playlist/copy";
            public const string PlaylistUser = "playlist/user";
        }
    }
}
