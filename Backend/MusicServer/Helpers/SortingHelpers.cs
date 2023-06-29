﻿using DataAccess.Entities;
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
                        return playlists.OrderBy(x => x.Name);
                    case SortingElementsPublicPlaylists.DateCreated:
                        return playlists.OrderBy(x => x.Created);
                    case SortingElementsPublicPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => x.Songs.Count());
                    default:
                        return playlists.OrderBy(x => x.Name);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsPublicPlaylists.Name:
                    return playlists.OrderByDescending(x => x.Name);
                case SortingElementsPublicPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => x.Created);
                case SortingElementsPublicPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => x.Songs.Count());
                default:
                    return playlists.OrderByDescending(x => x.Name);
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
                        return songs.OrderBy(x => x.Song.Name);
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name);
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => x.Song.Length);
                    case SortingElementsOwnPlaylistSongs.Custom:
                        return songs.OrderBy(x => x.Order);
                    default:
                        return songs.OrderBy(x => x.Song.Name);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => x.Song.Name);
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => x.Song.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name);
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => x.Song.Length);
                case SortingElementsOwnPlaylistSongs.Custom:
                    return songs.OrderByDescending(x => x.Order);
                default:
                    return songs.OrderByDescending(x => x.Song.Name);
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
                        return songs.OrderBy(x => x.FavoriteSong.Name);
                    case SortingElementsOwnPlaylistSongs.Artist:
                        return songs.OrderBy(x => x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name);
                    case SortingElementsOwnPlaylistSongs.Duration:
                        return songs.OrderBy(x => x.FavoriteSong.Length);
                    case SortingElementsOwnPlaylistSongs.Custom:
                        return songs.OrderBy(x => x.Order);
                    default:
                        return songs.OrderBy(x => x.FavoriteSong.Name);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylistSongs.Name:
                    return songs.OrderByDescending(x => x.FavoriteSong.Name);
                case SortingElementsOwnPlaylistSongs.Artist:
                    return songs.OrderByDescending(x => x.FavoriteSong.Artists.OrderBy(x => x.Artist.Name).First().Artist.Name);
                case SortingElementsOwnPlaylistSongs.Duration:
                    return songs.OrderByDescending(x => x.FavoriteSong.Length);
                case SortingElementsOwnPlaylistSongs.Custom:
                    return songs.OrderByDescending(x => x.Order);
                default:
                    return songs.OrderByDescending(x => x.FavoriteSong.Name);
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
                        return playlists.OrderBy(x => x.Playlist.Name);
                    case SortingElementsOwnPlaylists.DateCreated:
                        return playlists.OrderBy(x => x.Playlist.Created);
                    case SortingElementsOwnPlaylists.NumberOfSongs:
                        return playlists.OrderBy(x => x.Playlist.Songs.Count());
                    case SortingElementsOwnPlaylists.Custom:
                        return playlists.OrderBy(x => x.Order);
                    default:
                        return playlists.OrderBy(x => x.Playlist.Name);
                }
            }

            switch (sortAfter)
            {
                case SortingElementsOwnPlaylists.Name:
                    return playlists.OrderByDescending(x => x.Playlist.Name);
                case SortingElementsOwnPlaylists.DateCreated:
                    return playlists.OrderByDescending(x => x.Playlist.Created);
                case SortingElementsOwnPlaylists.NumberOfSongs:
                    return playlists.OrderByDescending(x => x.Playlist.Songs.Count());
                case SortingElementsOwnPlaylists.Custom:
                    return playlists.OrderByDescending(x => x.Order);
                default:
                    return playlists.OrderByDescending(x => x.Playlist.Name);
            }
        }


    }
}