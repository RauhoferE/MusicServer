using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class GuidNamePaginationResponse
    {
        public int TotalCount
        {
            get; set;
        }

        public ArtistShortDto[] Items { get; set; } = new ArtistShortDto[0];
    }
}
