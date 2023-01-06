namespace MusicServer.Entities.Requests.Song
{
    public class UpdatePlaylist
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
