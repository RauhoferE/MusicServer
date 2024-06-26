﻿using AutoMapper;
using DataAccess.Entities;
using MusicServer.Entities.DTOs;
using MusicServer.Entities.HubEntities;

namespace MusicServer.Mapper
{
    public class EntityToDto : Profile
    {
        public EntityToDto()
        {
            this.CreateMap<PlaylistSong, PlaylistSongDto>(MemberList.Destination)
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<UserSong, PlaylistSongDto>(MemberList.Destination)
    .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.FavoriteSong.Album))
    .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.FavoriteSong.Artists))
    .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.FavoriteSong.Length))
    .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.FavoriteSong.Modified))
    .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.FavoriteSong.Created))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.FavoriteSong.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.FavoriteSong.Name))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<PlaylistUser, PlaylistUserShortDto>(MemberList.Destination)
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Playlist.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Playlist.Name))
                    .ForMember(dest => dest.Users, opt => opt.MapFrom(ps => ps.Playlist.Users))
                    .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Playlist.Created))
                    .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Playlist.Songs.Count))
    .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(ps => ps.Playlist.IsPublic))
    .ForMember(dest => dest.IsModifieable, opt => opt.MapFrom(ps => ps.IsModifieable))
    .ForMember(dest => dest.ReceiveNotifications, opt => opt.MapFrom(ps => ps.ReceiveNotifications))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order))
    .ForMember(dest => dest.IsCreator, opt => opt.MapFrom(ps => ps.IsCreator))
    .ForMember(dest => dest.LastListened, opt => opt.MapFrom(ps => ps.LastListened))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(ps => ps.Playlist.Description));

            this.CreateMap<Playlist, PlaylistUserShortDto>(MemberList.Destination)
        .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Name))
        .ForMember(dest => dest.Users, opt => opt.MapFrom(ps => ps.Users))
        .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count))
.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(ps => ps.IsPublic))
.ForMember(dest => dest.IsModifieable, opt => opt.MapFrom(ps => false))
.ForMember(dest => dest.ReceiveNotifications, opt => opt.MapFrom(ps => false))
.ForMember(dest => dest.IsCreator, opt => opt.MapFrom(ps => false))
.ForMember(dest => dest.Order, opt => opt.MapFrom(ps => 0))
.ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Created))
.ForMember(dest => dest.Description, opt => opt.MapFrom(ps => ps.Description));

            this.CreateMap<PlaylistUser, UserDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.User.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(ps => ps.User.UserName))
                .ForMember(dest => dest.IsCreator, opt => opt.MapFrom(ps => ps.IsCreator));

            this.CreateMap<Artist, GuidNameDto>(MemberList.Destination);

            this.CreateMap<PlaylistUser, GuidNameDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Playlist.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Playlist.Name));

            this.CreateMap<ArtistSong, GuidNameDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Artist.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Artist.Name));

            this.CreateMap<ArtistSong, SongDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album));

            this.CreateMap<Album, AlbumArtistDto>(MemberList.Destination);

            this.CreateMap<Artist, ArtistDto>(MemberList.Destination);
                //.ForMember(dest => dest.Albums, opt => opt.MapFrom(ps => ps.Albums.Select(x => x.Album)))
                //.ForMember(dest => dest.Songs, opt => opt.MapFrom(ps => ps.Songs.Select(x => x.Song)));


            this.CreateMap<Album, AlbumDto>(MemberList.Destination)
                .ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Songs.Sum(x => x.Length)))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Artists.Select(x => x.Artist)));

            this.CreateMap<Song, SongDto>(MemberList.Destination)
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Length));

            this.CreateMap<Song, PlaylistSongDto>(MemberList.Destination)
    .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Length))
                    .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Album))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Artists))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => -1));

            this.CreateMap<User, UserDto>(MemberList.Destination);

            this.CreateMap<UserUser, LongNameDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.FollowedUser.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.FollowedUser.UserName));

            this.CreateMap<UserUser, GuidNameDto>(MemberList.Destination)
    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.FollowedUser.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.FollowedUser.UserName))
    .ForMember(dest => dest.ReceiveNotifications, opt => opt.MapFrom(ps => ps.ReceiveNotifications))
    .ForMember(dest => dest.FollowedByUser, opt => opt.MapFrom(ps => ps.FollowedUser));

            this.CreateMap<User, LongNameDto>(MemberList.Destination)
    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.UserName));

            this.CreateMap<Playlist, FollowedPlaylistDto>(MemberList.Destination)
.ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Id))
.ForMember(dest => dest.CreatorName, opt => opt.MapFrom(ps => string.Empty))
.ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Name))
.ForMember(dest => dest.SongCount, opt => opt.MapFrom(ps => ps.Songs.Count));

            this.CreateMap<Artist, LongNameDto>(MemberList.Destination)
.ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Id))
.ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Name));

            this.CreateMap<UserUser, UserDto>(MemberList.Destination)
    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.FollowedUser.Id))
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(ps => ps.FollowedUser.UserName))
    .ForMember(dest => dest.ReceiveNotifications, opt => opt.MapFrom(ps => ps.ReceiveNotifications))
    .ForMember(dest => dest.IsFollowedByUser, opt => opt.MapFrom(ps => true));

            this.CreateMap<UserArtist, GuidNameDto>(MemberList.Destination)
    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Artist.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Artist.Name))
    .ForMember(dest => dest.ReceiveNotifications, opt => opt.MapFrom(ps => ps.ReceiveNotifications))
    .ForMember(dest => dest.FollowedByUser, opt => opt.MapFrom(ps => false));

            this.CreateMap<User, FullUserDto>(MemberList.Destination);

            this.CreateMap<User, UserDetailsDto>(MemberList.Destination)
                .ForMember(dest => dest.FollowedArtists,
                opt => opt.MapFrom((ps, o, d, t) => t.Mapper.Map<GuidNameDto[]>(ps.FollowedArtists.ToArray())))
                .ForMember(dest => dest.FollowedUsers,
                opt => opt.MapFrom((ps, o, d, t) => t.Mapper.Map<LongNameDto[]>(ps.FollowedUsers.ToArray())))
                .ForMember(dest => dest.Playlists,
                opt => opt.MapFrom((ps, o, d, t) => t.Mapper.Map<PlaylistUserShortDto[]>(ps.Playlists.ToArray())))
                .ForMember(dest => dest.Roles,
                opt => opt.MapFrom((ps, o, d, t) => t.Mapper.Map<LongNameDto[]>(ps.UserRoles.ToArray())));

            this.CreateMap<UserRole, LongNameDto>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.RoleId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Role.Name));

            this.CreateMap<QueueEntity, PlaylistSongDto>(MemberList.Destination)
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<QueueEntity, QueueSongDto>(MemberList.Destination)
                                    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<QueueData, QueueDataDto>(MemberList.Destination)
    .ForMember(dest => dest.Target, opt => opt.MapFrom(ps => ps.Target.Name))
    .ForMember(dest => dest.LoopMode, opt => opt.MapFrom(ps => ps.LoopMode.Name))
    .ForMember(dest => dest.SortAfter, opt => opt.MapFrom(ps => ps.SortAfter.Name))
    .ForMember(dest => dest.UserId, opt => opt.MapFrom(ps => ps.UserId));

            this.CreateMap<GroupQueueEntity, PlaylistSongDto>(MemberList.Destination)
        .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
    .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
    .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
    .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
    .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
    .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
    .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
    .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
.ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<GroupQueueEntity, QueueSongDto>(MemberList.Destination)
                                    .ForMember(dest => dest.Id, opt => opt.MapFrom(ps => ps.Song.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(ps => ps.Song.Name))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(ps => ps.Song.Length))
                .ForMember(dest => dest.Modified, opt => opt.MapFrom(ps => ps.Song.Modified))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Created, opt => opt.MapFrom(ps => ps.Song.Created))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(ps => ps.Song.Artists))
                .ForMember(dest => dest.Album, opt => opt.MapFrom(ps => ps.Song.Album))
    .ForMember(dest => dest.Order, opt => opt.MapFrom(ps => ps.Order));

            this.CreateMap<GroupQueueData, QueueDataDto>(MemberList.Destination)
    .ForMember(dest => dest.Target, opt => opt.MapFrom(ps => ps.Target.Name))
    .ForMember(dest => dest.LoopMode, opt => opt.MapFrom(ps => ps.LoopMode.Name))
    .ForMember(dest => dest.SortAfter, opt => opt.MapFrom(ps => ps.SortAfter.Name))
    .ForMember(dest => dest.UserId, opt => opt.MapFrom(ps => ps.UserId));

            this.CreateMap<Group, SessionUserData>(MemberList.Destination)
                    .ForMember(dest => dest.UserId, opt => opt.MapFrom(ps => ps.UserId))
    .ForMember(dest => dest.IsMaster, opt => opt.MapFrom(ps => ps.IsMaster))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(ps => ps.Email));
        }
    }
}
