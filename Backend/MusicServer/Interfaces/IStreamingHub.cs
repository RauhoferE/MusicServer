using Microsoft.AspNetCore.Mvc;
using MusicServer.Entities.Requests.Song;
using System.ComponentModel.DataAnnotations;

namespace MusicServer.Interfaces
{
    public interface IStreamingHub
    {
        // Returns to the user who created it a unique Id
        public Task GetSessionId(Guid id);
    }
}
