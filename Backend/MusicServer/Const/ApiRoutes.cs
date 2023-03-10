namespace MusicServer.Const
{
    public static class ApiRoutes
    {
        private const string Root = "api";

        private const string Controller = "[controller]";

        public const string Base = Root + "/" + Controller;

        public static class User
        {
            public const string SubscribeUser = "subscribe/user/{userId}";
            public const string SubscribeArtist = "subscribe/artist/{artistId}";
            public const string GetFollowedArtists = "followed/artists";
            public const string GetFollowedUsers = "followed/users";
            public const string GetUsers = "all/users";
            public const string GetUser = "{userId}";
            public const string RoleUser = "role/{roleId}/{userId}";
            public const string Roles = "roles";
        }

        public static class Authentication
        {
            public const string Login = "login";
            public const string Logout = "logout";
            public const string Register = "register";
            public const string ConfirmMail = "confirm/email/{email}/{token}";
            public const string ChangePassword = "change/password";
            public const string ForgetPassword = "forget/password";
            public const string ResetPassword = "reset/password/{userId}/{token}";
            public const string RequestEmailReset = "reset/email";
            public const string ChangeEmail = "change/email/{userId}/{token}";
            public const string DeleteAccount = "delete/account";
            public const string GenerateRegistrationCodes = "generate/code/{amount}";
        }

        public static class Development
        {
            public const string Test = "test";
            public const string CreateArtistsAndSongs = "create/{artists}/{albums}/{songs}";
        }

        public static class Playlist
        {
            public const string Default = "";
            public const string Songs = "songs/{playlistId}";
            public const string Playlists = "playlists";
            public const string UserPlaylists = "user/{userId}";
            public const string PublicPlaylist = "public";
            public const string PlaylistAlbum = "album/{playlistId}";
            public const string PlaylistShare = "share/{playlistId}";
            public const string PlaylistCopy = "copy/{playlistId}";
            public const string PlaylistAddToLibrary = "add/{playlistId}";
            public const string Favorite = "favorites";
        }

        public static class Song
        {
            public const string Artist = "artist/{artistId}";
            public const string ArtistAlbums = "artist/{artistId}/albums";
            public const string SongsInAlbum = "album/{albumId}";
            public const string SongDefault = "{songId}";
            public const string Search = "search";
        }
    }
}
