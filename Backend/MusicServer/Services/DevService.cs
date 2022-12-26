using DataAccess;
using DataAccess.Entities;
using MusicServer.Interfaces;

namespace MusicServer.Services
{
    public class DevService : IDevService
    {
        private readonly MusicServerDBContext dBContext;

        public DevService(MusicServerDBContext dBContext)
        {
            this.dBContext = dBContext;
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
    }
}
