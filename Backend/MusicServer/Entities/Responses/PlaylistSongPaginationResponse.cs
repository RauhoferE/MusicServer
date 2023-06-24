using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class PlaylistSongPaginationResponse
    {
        public int TotalCount { get; set; }

        public PlaylistSongDto[] Songs { get; set; } = new PlaylistSongDto[0];
    }
}
