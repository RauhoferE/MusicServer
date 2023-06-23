using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class PlaylistPaginationResponse
    {
        public int TotalCount { get; set; }

        public PlaylistShortDto[] Playlists { get; set; } = new PlaylistShortDto[0];
    }
}
