namespace MusicServer.Entities.DTOs
{
    public class SearchResultDto
    {
        public PlaylistShortDto[] Playlists { get; set; }

        public GuidNameDto[] Artists { get; set; }

        public SongDto[] Songs { get; set; }

        public AlbumDto[] Albums { get;set; }

        public UserDto[] Users { get; set; }
    }
}
