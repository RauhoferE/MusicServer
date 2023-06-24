using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class FullUserPaginationResponse
    {
        public int TotalCount { get; set; }

        public FullUserDto[] Users { get; set; } = new FullUserDto[0];
    }
}
