namespace MusicServer.Entities.Requests.Song
{
    public class SongPaginationSearchRequest
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string Query { get; set; } = null;

        public string SortAfter { get; set; } = null;

        public bool Asc { get; set; } = true;
    }
}
