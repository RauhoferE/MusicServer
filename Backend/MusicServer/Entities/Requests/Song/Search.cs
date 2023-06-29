using MusicServer.Entities.Requests.Multi;

namespace MusicServer.Entities.Requests.Song
{
    public class Search : QueryPaginationSearchRequest
    {
        public string Filter { get; set; }

        public string SearchTerm { get; set; }
    }
}
