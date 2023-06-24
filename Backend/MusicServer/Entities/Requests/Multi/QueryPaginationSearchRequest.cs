using Microsoft.AspNetCore.Mvc;

namespace MusicServer.Entities.Requests.Multi
{
    public class QueryPaginationSearchRequest
    {
        public int Page { get; set; }

        public int Take { get; set; }

        public string Query { get; set; } = null;
    }
}
