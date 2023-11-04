namespace MusicServer.Entities.DTOs
{
    public class SongStreamDto
    {
        public Guid Id { get; set; }

        public bool IsHalted { get; set; }

        public double AtSecond { get; set; }
    }
}
