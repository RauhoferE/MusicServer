using DataAccess.Entities;
using MusicServer.Core.Const;

namespace MusicServer.Helpers
{
    public static class SortingHelpers
    {
        public static IQueryable<Playlist> SortSearchPublicPlaylists(IQueryable<Playlist> playlists, bool asc, string sortAfter, string query)
        {
            if(query != null)
            {
                  playlists = playlists
                    .Where(x => x.Name.Contains(query));
            }
            
            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsPublicPlaylists.Name:
                        return playlists.OrderBy(x => x.Name).ThenBy(x => x.Id);
                    case SortingElementsPublicPlaylists.DateCreated:
                        return playlists.OrderBy(x => x.Created).ThenBy(x => x.Id);
                    case SortingElementsPublicPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => x.Songs.Count()).ThenBy(x => x.Id);
                    default:
                        return playlists.OrderBy(x => x.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsPublicPlaylists.Name:
                    return playlists.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
                case SortingElementsPublicPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => x.Created).ThenBy(x => x.Id);
                case SortingElementsPublicPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => x.Songs.Count()).ThenBy(x => x.Id);
                default:
                    return playlists.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
            }
        }

        public static IQueryable<PlaylistSong> SortSearchSongsInPlaylist(IQueryable<PlaylistSong> songs, bool asc, string sortAfter, string query)
        {
            if (query != null)
            {
                songs = songs
                  .Where(x => x.Song.Name.Contains(query) ||
                    x.Song.Artists.Where(x => x.Artist.Name.Contains(query)).Any());
            }

            if (sortAfter != null)
            {
                sortAfter = sortAfter.ToLower();
            }

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsOwnPlaylistSongs.Name:
                        return songs.OrderBy(x => x.Song.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => x.Song.Length).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Order:
                        return songs.OrderBy(x => x.Order).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Album:
                        return songs.OrderBy(x => x.Song.Album.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.DateAdded:
                        return songs.OrderBy(x => x.Added).ThenBy(x => x.Id);
                    default:
                        return songs.OrderBy(x => x.Song.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => x.Song.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => x.Song.Length).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Order:
                    return songs.OrderByDescending(x => x.Order).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Album:
                    return songs.OrderByDescending(x => x.Song.Album.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.DateAdded:
                    return songs.OrderByDescending(x => x.Added).ThenBy(x => x.Id);
                default:
                    return songs.OrderByDescending(x => x.Song.Name).ThenBy(x => x.Id);
            }
        }

        public static IQueryable<UserSong> SortSearchFavorites(IQueryable<UserSong> songs, bool asc, string sortAfter, string query)
        {
            if (query != null)
            {
                songs = songs
                  .Where(x => x.FavoriteSong.Name.ToLower().Contains(query.ToLower()) ||
                        x.FavoriteSong.Artists.Where(x => x.Artist.Name.ToLower().Contains(query.ToLower())).Any());
            }

            if (sortAfter != null)
            {
                sortAfter = sortAfter.ToLower();
            }

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsOwnPlaylistSongs.Name:
                        return songs.OrderBy(x => x.FavoriteSong.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => x.FavoriteSong.Length).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.Order:
                        return songs.OrderBy(x => x.Order);
                    case SortingElementsOwnPlaylistSongs.Album:
                        return songs.OrderBy(x => x.FavoriteSong.Album.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylistSongs.DateAdded:
                        return songs.OrderBy(x => x.Created).ThenBy(x => x.Id);
                    default:
                        return songs.OrderBy(x => x.FavoriteSong.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => x.FavoriteSong.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => x.FavoriteSong.Length).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.Order:
                    return songs.OrderByDescending(x => x.Order);
                case SortingElementsOwnPlaylistSongs.Album:
                    return songs.OrderByDescending(x => x.FavoriteSong.Album.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylistSongs.DateAdded:
                    return songs.OrderByDescending(x => x.Created).ThenBy(x => x.Id);
                default:
                    return songs.OrderByDescending(x => x.FavoriteSong.Name).ThenBy(x => x.Id);
            }
        }

        public static IQueryable<PlaylistUser> SortSearchUserPlaylists(IQueryable<PlaylistUser> playlists, bool asc, string sortAfter, string query)
        {
            if (query != null)
            {
                playlists = playlists
                  .Where(x => x.Playlist.Name.ToLower().Contains(query.ToLower()));
            }

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsOwnPlaylists.Name:
                        return playlists.OrderBy(x => x.Playlist.Name).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylists.DateCreated:
                        return playlists.OrderBy(x => x.Playlist.Created).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => x.Playlist.Songs.Count()).ThenBy(x => x.Id);
                    case SortingElementsOwnPlaylists.Custom:
                        return playlists.OrderBy(x => x.Order).ThenBy(x => x.Id);
                    default:
                        return playlists.OrderBy(x => x.Playlist.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylists.Name:
                    return playlists.OrderByDescending(x => x.Playlist.Name).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => x.Playlist.Created).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => x.Playlist.Songs.Count()).ThenBy(x => x.Id);
                case SortingElementsOwnPlaylists.Custom:
                    return playlists.OrderByDescending(x => x.Order).ThenBy(x => x.Id);
                default:
                    return playlists.OrderByDescending(x => x.Playlist.Name).ThenBy(x => x.Id);
            }
        }

        public static IQueryable<Album> SortSearchAlbums(IQueryable<Album> albums, bool asc, string sortAfter, string query)
        {
            if (query != null)
            {
                albums = albums
                  .Where(x => x.Name.Contains(query));
            }

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsAllAlbums.Name:
                        return albums.OrderBy(x => x.Name).ThenBy(x => x.Id);
                    case SortingElementsAllAlbums.DateAdded:
                        return albums.OrderBy(x => x.Created).ThenBy(x => x.Id);
                    case SortingElementsAllAlbums.NumberOfSongs:
                        return albums.OrderBy(x => x.Songs.Count()).ThenBy(x => x.Id);
                    case SortingElementsAllAlbums.Artist:
                        return albums.OrderBy(x => x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                    default:
                        return albums.OrderBy(x => x.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsAllAlbums.Name:
                    return albums.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
                case SortingElementsAllAlbums.DateAdded:
                    return albums.OrderByDescending(x => x.Created).ThenBy(x => x.Id);
                case SortingElementsAllAlbums.NumberOfSongs:
                    return albums.OrderByDescending(x => x.Songs.Count()).ThenBy(x => x.Id);
                case SortingElementsAllAlbums.Artist:
                    return albums.OrderByDescending(x => x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                default:
                    return albums.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
            }
        }

        public static IQueryable<Artist> SortSearchArtists(IQueryable<Artist> artists, bool asc, string query)
        {
            if (query != null)
            {
                artists = artists
                  .Where(x => x.Name.Contains(query));
            }

            if (asc)
            {
                return artists.OrderBy(x => x.Name).ThenBy(x => x.Id);
            }

            return artists.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
        }

        public static IQueryable<UserArtist> SortSearchFollowedArtists(IQueryable<UserArtist> artists, string query)
        {
            if (query != null)
            {
                query = query.ToLower();
                artists = artists
                  .Where(x => x.Artist.Name.ToLower().Contains(query));
            }

            return artists.OrderBy(x => x.Artist.Name).ThenBy(x => x.Id);
        }

        public static IQueryable<User> SortSearchUsers(IQueryable<User> users, bool asc, string query)
        {
            if (query != null)
            {
                users = users
                  .Where(x => x.UserName.Contains(query) || x.Email.Contains(query));
            }

            if (asc)
            {
                return users.OrderBy(x => x.UserName).ThenBy(x => x.Id);
            }

            return users.OrderByDescending(x => x.UserName).ThenBy(x => x.Id);
        }

        public static IQueryable<UserUser> SortSearchFollowedUsers(IQueryable<UserUser> users, string query)
        {
            if (query != null)
            {
                query = query.ToLower();
                users = users
                  .Where(x => x.FollowedUser.UserName.ToLower().Contains(query) || x.FollowedUser.Email.Contains(query));
            }

            return users.OrderBy(x => x.FollowedUser.UserName).ThenBy(x => x.Id);
        }

        public static IQueryable<Song> SortSearchAllSongs(IQueryable<Song> songs, bool asc, string query, string sortAfter)
        {
            if (query != null)
            {
                songs = songs
                  .Where(x => x.Name.Contains(query));
            }

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsAllSongs.Name:
                        return songs.OrderBy(x => x.Name).ThenBy(x => x.Id);
                    case SortingElementsAllSongs.DateAdded:
                        return songs.OrderBy(x => x.Created).ThenBy(x => x.Id);
                    case SortingElementsAllSongs.Duration:
                        return songs.OrderBy(x => x.Length).ThenBy(x => x.Id);
                    case SortingElementsAllSongs.Artist:
                        return songs.OrderBy(x => x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                    default:
                        return songs.OrderBy(x => x.Name).ThenBy(x => x.Id);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsAllSongs.Name:
                    return songs.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
                case SortingElementsAllSongs.DateAdded:
                    return songs.OrderByDescending(x => x.Created).ThenBy(x => x.Id);
                case SortingElementsAllSongs.Duration:
                    return songs.OrderByDescending(x => x.Length).ThenBy(x => x.Id);
                case SortingElementsAllSongs.Artist:
                    return songs.OrderByDescending(x => x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name).ThenBy(x => x.Id);
                default:
                    return songs.OrderByDescending(x => x.Name).ThenBy(x => x.Id);
            }
        }


    }
}
