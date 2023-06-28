using DataAccess.Entities;
using MusicServer.Core.Const;

namespace MusicServer.Helpers
{
    public static class SortingHelpers
    {
        public static IQueryable<Playlist> SortSearchPublicPlaylists(string sortAfter, IQueryable<Playlist> playlists, bool asc)
        {
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


    }
}
