using AutoMapper;
using DataAccess.Entities;
using MusicServer.Entities.Requests.User;

namespace MusicServer.Mapper
{
    public class RequestToDto : Profile
    {
        public RequestToDto()
        {
            CreateMap<Register, User>();
        }
    }
}
