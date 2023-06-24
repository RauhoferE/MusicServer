using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class LongNamePaginationResponse
    {
        public int TotalCount { get; set; }

        public LongNameDto[] Items { get; set; } = new LongNameDto[0];
    }
}
