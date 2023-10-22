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
                        return playlists.OrderBy(x => new { x.Name, x.Id });
                    case SortingElementsPublicPlaylists.DateCreated:
                        return playlists.OrderBy(x => new { x.Created, x.Id });
                    case SortingElementsPublicPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => new { x.Songs.Count, x.Id });
                    default:
                        return playlists.OrderBy(x => new { x.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsPublicPlaylists.Name:
                    return playlists.OrderByDescending(x => new { x.Name, x.Id });
                case SortingElementsPublicPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => new { x.Created, x.Id });
                case SortingElementsPublicPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => new { x.Songs.Count, x.Id });
                default:
                    return playlists.OrderByDescending(x => new { x.Name, x.Id });
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

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsOwnPlaylistSongs.Name:
                        return songs.OrderBy(x => new { x.Song.Name, x.Id });
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => new { x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => new { x.Song.Length, x.Id });
                    case SortingElementsOwnPlaylistSongs.Custom:
                        return songs.OrderBy(x => new { x.Order, x.Id });
                    default:
                        return songs.OrderBy(x => new { x.Song.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => new { x.Song.Name, x.Id });
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => new { x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => new { x.Song.Length, x.Id });
                case SortingElementsOwnPlaylistSongs.Custom:
                    return songs.OrderByDescending(x => new { x.Order, x.Id });
                default:
                    return songs.OrderByDescending(x => new { x.Song.Name, x.Id });
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

            if (asc)
            {
                switch (sortAfter)
                {
                    case SortingElementsOwnPlaylistSongs.Name:
                        return songs.OrderBy(x => new { x.FavoriteSong.Name, x.Id });
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => new { x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => new { x.FavoriteSong.Length, x.Id });
                    case SortingElementsOwnPlaylistSongs.Custom:
                        return songs.OrderBy(x => new { x.Order, x.Id });
                    default:
                        return songs.OrderBy(x => new { x.FavoriteSong.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => new { x.FavoriteSong.Name, x.Id });
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => new { x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => new { x.FavoriteSong.Length, x.Id });
                case SortingElementsOwnPlaylistSongs.Custom:
                    return songs.OrderByDescending(x => new { x.Order, x.Id });
                default:
                    return songs.OrderByDescending(x => new { x.FavoriteSong.Name, x.Id });
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
                        return playlists.OrderBy(x => new { x.Playlist.Name, x.Id });
                    case SortingElementsOwnPlaylists.DateCreated:
                        return playlists.OrderBy(x => new { x.Playlist.Created, x.Id });
                    case SortingElementsOwnPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => new { x.Playlist.Songs.Count, x.Id });
                    case SortingElementsOwnPlaylists.Custom:
                        return playlists.OrderBy(x => new { x.Order, x.Id });
                    default:
                        return playlists.OrderBy(x => new { x.Playlist.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylists.Name:
                    return playlists.OrderByDescending(x => new { x.Playlist.Name, x.Id });
                case SortingElementsOwnPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => new { x.Playlist.Created, x.Id });
                case SortingElementsOwnPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => new { x.Playlist.Songs.Count, x.Id });
                case SortingElementsOwnPlaylists.Custom:
                    return playlists.OrderByDescending(x => new { x.Order, x.Id });
                default:
                    return playlists.OrderByDescending(x => new { x.Playlist.Name, x.Id });
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
                        return albums.OrderBy(x => new { x.Name, x.Id });
                    case SortingElementsAllAlbums.DateAdded:
                        return albums.OrderBy(x => new { x.Created, x.Id });
                    case SortingElementsAllAlbums.NumberOfSongs:
                        return albums.OrderBy(x => new { x.Songs.Count, x.Id });
                    case SortingElementsAllAlbums.Artist:
                        return albums.OrderBy(x => new { x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                    default:
                        return albums.OrderBy(x => new { x.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsAllAlbums.Name:
                    return albums.OrderByDescending(x => new { x.Name, x.Id });
                case SortingElementsAllAlbums.DateAdded:
                    return albums.OrderByDescending(x => new { x.Created, x.Id });
                case SortingElementsAllAlbums.NumberOfSongs:
                    return albums.OrderByDescending(x => new { x.Songs.Count, x.Id });
                case SortingElementsAllAlbums.Artist:
                    return albums.OrderByDescending(x => new { x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                default:
                    return albums.OrderByDescending(x => new { x.Name, x.Id });
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
                return artists.OrderBy(x => new { x.Name, x.Id });
            }

            return artists.OrderByDescending(x => new { x.Name, x.Id });
        }

        public static IQueryable<UserArtist> SortSearchFollowedArtists(IQueryable<UserArtist> artists, bool asc, string query)
        {
            if (query != null)
            {
                query = query.ToLower();
                artists = artists
                  .Where(x => x.Artist.Name.ToLower().Contains(query));
            }

            if (asc)
            {
                return artists.OrderBy(x => new { x.Artist.Name, x.Id });
            }

            return artists.OrderByDescending(x => new { x.Artist.Name, x.Id });
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
                return users.OrderBy(x => new { x.UserName, x.Id });
            }

            return users.OrderByDescending(x => new { x.UserName, x.Id });
        }

        public static IQueryable<UserUser> SortSearchFollowedUsers(IQueryable<UserUser> users, bool asc, string query)
        {
            if (query != null)
            {
                query = query.ToLower();
                users = users
                  .Where(x => x.FollowedUser.UserName.ToLower().Contains(query) || x.FollowedUser.Email.Contains(query));
            }

            if (asc)
            {
                return users.OrderBy(x => new { x.FollowedUser.UserName, x.Id });
            }

            return users.OrderByDescending(x => new { x.FollowedUser.UserName, x.Id });
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
                        return songs.OrderBy(x => new { x.Name, x.Id });
                    case SortingElementsAllSongs.DateAdded:
                        return songs.OrderBy(x => new { x.Created, x.Id });
                    case SortingElementsAllSongs.Duration:
                        return songs.OrderBy(x => new { x.Length, x.Id });
                    case SortingElementsAllSongs.Artist:
                        return songs.OrderBy(x => new { x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                    default:
                        return songs.OrderBy(x => new { x.Name, x.Id });
                }
            }

            switch (sortAfter)
            {
                case SortingElementsAllSongs.Name:
                    return songs.OrderByDescending(x => new { x.Name, x.Id });
                case SortingElementsAllSongs.DateAdded:
                    return songs.OrderByDescending(x => new { x.Created, x.Id });
                case SortingElementsAllSongs.Duration:
                    return songs.OrderByDescending(x => new { x.Length, x.Id });
                case SortingElementsAllSongs.Artist:
                    return songs.OrderByDescending(x => new { x.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name, x.Id });
                default:
                    return songs.OrderByDescending(x => new { x.Name, x.Id });
            }
        }


    }
}
