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
    public class MusicServerDBContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, RoleClaim, IdentityUserToken<Guid>>
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
                .WithOne(p => p.User);

                u.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            builder.Entity<Artist>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Albums)
                    .WithOne(p => p.Artist);

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Artist);
            });

            builder.Entity<Album>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Album);

                entity.HasMany(e => e.Artists)
                    .WithOne(p => p.Album);
            });

            builder.Entity<Song>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Artists)
                    .WithOne(p => p.Song);

                entity.HasOne(e => e.Album);

                entity.HasMany(e => e.Playlists)
    .WithOne(p => p.Song);
            });

            builder.Entity<Playlist>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();

                entity.HasMany(e => e.Songs)
                    .WithOne(p => p.Playlist);

                entity.HasMany(e => e.Users)
                .WithOne(p => p.Playlist);
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
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                        Name = RolesEnum.Root,
                        NormalizedName = RolesEnum.Root.ToUpper(),
                    },
                    new Role()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                        Name = RolesEnum.Admin,
                        NormalizedName = RolesEnum.Admin.ToUpper(),
                    },
                    new Role()
                    {
                        Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                        Name = RolesEnum.User,
                        NormalizedName = RolesEnum.User.ToUpper(),
                    }
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
        }
    }
}
