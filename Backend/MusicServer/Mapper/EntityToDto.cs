using AutoMapper;
using DataAccess.Entities;
using MusicServer.Entities.DTOs;

namespace MusicServer.Mapper
{
    public class EntityToDto : Profile
    {
        public EntityToDto()
        {
            this.CreateMap<PlaylistSong, SongDto>(MemberList.Destination)
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album.Name))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists.Select(x => x.Artist.Name)))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name));

            this.CreateMap<PlaylistUser, PlaylistUserDto>(MemberList.Destination)
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(ps => ps.User.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(ps => ps.User.UserName))
                .ForMember(dest => dest.IsModifieable, opt => opt.MapFrom(ps => ps.IsModifieable));

            this.CreateMap<Playlist, PlaylistDto>(MemberList.Destination)
                .ForMember(dest => dest.Songs, opt => opt.MapFrom(ps => ps.Songs))
                .ForMember(dest => dest.Users, opt => opt.MapFrom(ps => ps.Users));
        }
    }
}
