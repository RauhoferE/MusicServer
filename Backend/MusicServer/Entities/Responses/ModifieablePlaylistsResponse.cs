using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class ModifieablePlaylistsResponse
    {
        public GuidNameDto[] Playlists { get; set; }
    }
}
