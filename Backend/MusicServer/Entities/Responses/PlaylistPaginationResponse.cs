using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class PlaylistPaginationResponse
    {
        public int TotalCount { get; set; }

        public PlaylistUserShortDto[] Playlists { get; set; } = new PlaylistUserShortDto[0];
    }
}
