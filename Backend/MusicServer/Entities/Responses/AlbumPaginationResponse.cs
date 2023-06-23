using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class AlbumPaginationResponse
    {
        public int TotalCount { get; set; }

        public AlbumDto[] Albums { get; set; } = new AlbumDto[0];
    }
}
