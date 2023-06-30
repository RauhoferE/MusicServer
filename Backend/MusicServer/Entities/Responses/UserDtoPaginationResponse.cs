using MusicServer.Entities.DTOs;

namespace MusicServer.Entities.Responses
{
    public class UserDtoPaginationResponse
    {
        public int TotalCount { get; set; } 

        public UserDto[] Users { get; set; } = new UserDto[0];
    }
}
