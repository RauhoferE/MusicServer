namespace MusicServer.Entities.DTOs
{
    public class FavoriteDto
    {
           public int SongCount { get; set; }

        public SongDto[] Songs { get; set; }
    }
}
