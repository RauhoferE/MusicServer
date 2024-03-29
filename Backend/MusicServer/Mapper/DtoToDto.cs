using AutoMapper;
using DataAccess.Entities;
using MusicServer.Entities.DTOs;

namespace MusicServer.Mapper
{
    public class DtoToDto : Profile
    {
        public DtoToDto()
        {
            this.CreateMap<SongDto, PlaylistSongDto>(MemberList.Destination)
              .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => -1));

            this.CreateMap<QueueEntity, GroupQueueEntity>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            this.CreateMap<QueueData, GroupQueueData>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
