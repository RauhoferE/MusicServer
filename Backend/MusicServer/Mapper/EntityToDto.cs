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
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
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
                .ForMember(dest => dest.Users, opt => opt.MapFrom(ps => ps.Users))
                .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count));

            this.CreateMap<Playlist, PlaylistShortDto>(MemberList.Destination)
                .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count));

            this.CreateMap<PlaylistSong, SongDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Playlist.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Playlist.Name))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified));

            this.CreateMap<Artist, GuidNameDto>(MemberList.Destination);

            this.CreateMap<ArtistSong, GuidNameDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Artist.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Artist.Name));

            this.CreateMap<ArtistSong, SongArtistDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(ps => ps.Song.Length));

            this.CreateMap<Album, AlbumArtistDto>(MemberList.Destination);


            this.CreateMap<Album, AlbumDto>(MemberList.Destination)
                .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Songs.Sum(x => x.Length)))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Artists));

            this.CreateMap<Song, SongDto>(MemberList.Destination)
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Album))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Artists));
        }
    }
}
