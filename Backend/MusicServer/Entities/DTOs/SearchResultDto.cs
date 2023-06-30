namespace MusicServer.Entities.DTOs
{
    public class SearchResultDto
    {
        public PlaylistUserShortDto[] Playlists { get; set; } = new PlaylistUserShortDto[0];

        public GuidNameDto[] Artists { get; set; } = new GuidNameDto[0];

        public SongDto[] Songs { get; set; } = new SongDto[0];

        public AlbumDto[] Albums { get;set; } = new AlbumDto[0];

        public UserDto[] Users { get; set; } = new UserDto[0];
    }
}
