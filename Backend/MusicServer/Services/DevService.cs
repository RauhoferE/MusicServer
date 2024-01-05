using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class DevService : IDevService
    {
        private readonly MusicServerDBContext dBContext;
        private readonly UserManager<User> userManager;

        public DevService(MusicServerDBContext dBContext, UserManager<User> userManager)
        {
            this.dBContext = dBContext;
            this.userManager = userManager;
        }

        public async Task AddMoqArtistsAlbumsSongsAsync(int numberOfArtists, int numberOfAlbums, int numberOfSongs)
        {
            List<Artist> artists = new List<Artist>();
            var startAlbumDate = DateTime.Now.AddYears(-30);
            var endAlbumDate = DateTime.Now;

            // Create number of Artists
            for (int i = 0; i < numberOfArtists; i++)
            {
                artists.Add(new Artist()
                {
                    Description = FakerDotNet.Faker.Lorem.Sentence(),
                    Name = FakerDotNet.Faker.Music.Band(),
                });
            }

            foreach (var artist in artists)
            {
                // Create number of Albums for artist
                for (int i = 0; i < numberOfAlbums; i++)
                {
                    List<Song> songs = new List<Song>();

                    // Create number of songs for album
                    for (int j = 0; j < numberOfSongs; j++)
                    {
                        var songVariable = new Song()
                        {
                            Name = FakerDotNet.Faker.Internet.Username(),
                            Length = FakerDotNet.Faker.Number.Positive()
                        };
                        songVariable.Artists.Add(new ArtistSong()
                        {
                            Artist= artist
                        });

                        songs.Add(songVariable);
                    }
                    
                    artist.Albums.Add(new ArtistAlbum()
                    {
                        Added = DateTime.Now,
                        Album = new Album()
                        {
                            Description = FakerDotNet.Faker.Lorem.Sentence(),
                            Name = FakerDotNet.Faker.Music.Album(),
                            IsSingle = FakerDotNet.Faker.Boolean.Boolean(),
                            Release = FakerDotNet.Faker.Date.Between(startAlbumDate, endAlbumDate),
                            Songs = songs,
                        }
                    });
                }
            }

            await this.dBContext.AddRangeAsync(artists);
            await this.dBContext.SaveChangesAsync();
        }

        public async Task AddMoqUsersAndPlaylistsAsync(int numberOfUsers, int numberOfPlaylists)
        {
            var emailList = new List<string>();
            var rnd = new Random();

            // Create number of Users
            for (int i = 0; i < numberOfUsers; i++)
            {
                var email = rnd.Next().ToString() + FakerDotNet.Faker.Internet.Email();
                var succ = await this.userManager.CreateAsync(new User()
                {
                    UserName = FakerDotNet.Faker.Internet.Username(),
                    Birth = FakerDotNet.Faker.Date.Birthday(),
                    Email = email,
                }, "KillMe123123!!");

                if (succ.Succeeded)
                {
                    emailList.Add(email);
                }
            }

            foreach (var e in emailList)
            {
                var user = this.dBContext.Users.FirstOrDefault(x => x.Email.ToLower() == e.ToLower());
                user.EmailConfirmed = true;

                // Create the Playlists for the User
                for (int i = 0; i < numberOfPlaylists; i++)
                {
                    // Take random songs for the playlists
                    var rndSongs = this.dBContext.Songs.OrderBy(x => x.Id).Take(rnd.Next(1, 10)).ToList();
                    var pl = new Playlist()
                    {
                        Name = FakerDotNet.Faker.Pokemon.Name(),
                        Modified = DateTime.Now,
                        Created= DateTime.Now,
                        Songs = new List<PlaylistSong>()
                    };

                    for (int k = 0; k < rndSongs.Count; k++)
                    {
                        pl.Songs.Add(new PlaylistSong()
                        {
                            Added = DateTime.Now,
                            Order = k,
                            Song = rndSongs[k]
                        });
                    }

                    user.Playlists.Add(new PlaylistUser()
                    {
                        User = user,
                        Playlist = pl,
                        Added= DateTime.Now,
                        IsCreator= true,
                        IsModifieable= true,
                        Order = i,
                    });
                }
            }

            await this.dBContext.SaveChangesAsync();
        }
    }
}
