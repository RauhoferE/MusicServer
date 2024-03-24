namespace MusicServer.Entities.HubEntities
{
    public class CurrentPlayerData
    {
        public Guid CurrentSong { get; set; }

        public Guid ItemId { get; set; }

        public bool IsPlaying { get; set; } 

        public double SecondsPlayed { get; set; }

        public bool Random { get; set; }

        public string LoopMode { get; set; }
      
    }
}
