using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MusicServer.Core.Const;

namespace DataAccess
{
    public class MusicServerDBContext : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, RoleClaim, IdentityUserToken<long>>
    {
        public MusicServerDBContext(DbContextOptions<MusicServerDBContext> options) : base(options)
        {
        }

        public DbSet<Playlist> Playlists { get; set; }

        public DbSet<Artist> Artists { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Song> Songs { get; set; }

        public DbSet<ArtistAlbum> ArtistAlbums { get; set;}

        public DbSet<ArtistSong> ArtistSongs { get; set;}

        public DbSet<PlaylistSong> PlaylistSongs { get; set; }

        public DbSet<PlaylistUser> PlaylistUsers { get; set; }

        public DbSet<UserArtist> FollowedArtists { get; set; }

        public DbSet<UserUser> FollowedUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(u =>
            {
                u.HasKey(n => n.Id);
                u.Property(n => n.Id).ValueGeneratedOnAdd();

                u.HasMany(e => e.FollowedArtists);
                u.HasMany(e => e.Favorites);
                u.HasMany(e => e.FollowedUsers);
                
                u.HasMany(e => e.Playlists)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);

                u.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

                u.HasMany(e => e.FollowedUsers)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

                u.HasMany(e => e.FollowedArtists)
                            .WithOne(e => e.User)
                            .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Artist>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Albums)
                    .WithOne(p => p.Artist)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Artist)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Album)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Artists)
                    .WithOne(p => p.Album)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Song>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Artists)
                    .WithOne(p => p.Song)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Album);

                entity.HasMany(e => e.Playlists)
                    .WithOne(p => p.Song)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Playlist>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Playlist)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.Users)
                .WithOne(p => p.Playlist)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Role>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.UserRoles)
                    .WithOne(p => p.Role);

                entity.HasMany(e => e.RoleClaims)
                .WithOne(p => p.Role);

                entity.HasData(
                    new Role()
                    {
                        Id = (long)MusicServer.Core.Const.Roles.Root,
                        Name = MusicServer.Core.Const.Roles.Root.ToString(),
                        NormalizedName = MusicServer.Core.Const.Roles.Root.ToString().ToUpper(),
                    },
                    new Role()
                    {
                        Id = (long)MusicServer.Core.Const.Roles.Admin,
                        Name = MusicServer.Core.Const.Roles.Admin.ToString(),
                        NormalizedName = MusicServer.Core.Const.Roles.Admin.ToString().ToUpper(),
                    }//,
                    //new Role()
                    //{
                    //    Id = (long)MusicServer.Core.Const.Roles.User,
                    //    Name = MusicServer.Core.Const.Roles.User.ToString(),
                    //    NormalizedName = MusicServer.Core.Const.Roles.User.ToString().ToUpper(),
                    //}
                    );
            });

            builder.Entity<RoleClaim>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Role).WithMany(e => e.RoleClaims).HasForeignKey(e => new {e.RoleId});

                
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(n => new {n.RoleId, n.UserId});

                entity.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => new { e.RoleId });
                entity.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => new { e.UserId });


            });

            builder.Entity<ArtistAlbum>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Artist);

                entity.HasOne(e => e.Album);
            });

            builder.Entity<ArtistSong>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Artist);

                entity.HasOne(e => e.Song);
            });

            builder.Entity<PlaylistSong>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Playlist);

                entity.HasOne(e => e.Song);
            });

            builder.Entity<PlaylistUser>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.Playlist);

                entity.HasOne(e => e.User);
            });

            builder.Entity<UserArtist>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.User);

                entity.HasOne(e => e.Artist);
            });

            builder.Entity<UserUser>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.User);

                entity.HasOne(e => e.User);
            });
        }
    }
}
