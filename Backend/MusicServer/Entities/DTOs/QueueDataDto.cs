using DataAccess.Entities;

namespace MusicServer.Entities.DTOs
{
    public class QueueDataDto
    {
        public bool Asc { get; set; }

        public bool Random { get; set; }

        public string Target { get; set; }

        public string LoopMode { get; set; }

        public string SortAfter { get; set; }

        public Guid ItemId { get; set; }

        public long UserId { get; set; }
    }
}
