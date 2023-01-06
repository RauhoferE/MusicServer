using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Requests.Song
{
    public class PlaylistShareList
    {
        public UserPlaylistModifieable[] UserList { get; set; }
    }
}
