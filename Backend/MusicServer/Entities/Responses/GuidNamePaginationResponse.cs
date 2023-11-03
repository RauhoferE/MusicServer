using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class GuidNamePaginationResponse
    {
        public int TotalCount
        {
            get; set;
        }

        public GuidNameDto[] Items { get; set; } = new GuidNameDto[0];
    }
}
