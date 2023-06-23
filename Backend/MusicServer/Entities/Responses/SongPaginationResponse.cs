using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class SongPaginationResponse
    {
        public int TotalCount { get; set; }

        public SongDto[] Songs { get; set; } = new SongDto[0];
    }
}
