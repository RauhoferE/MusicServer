using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IMusicMailService
    {
        Task SendWelcomeEmailAsync(User user, string activationlink);

        Task SendPasswordResetEmailAsync(User user, string resetlink);

        Task SendEmailChangeEmailAsync(User user, string changeMailLink);

        Task SendPlaylistAddedFromUserEmailAsync(User user, Playlist playlist, User addedUser);

        Task SendPlaylistSharedWithUserEmailAsync(User user, Playlist playlist, User sharedUser);

        Task SendPlaylistRemovedFromUserEmailAsync(User user, Playlist playlist, User removedUser);

        Task SendTracksAddedFromArtistEmailAsync(User user, Artist artist, List<Song> songs);

        Task SendNewArtistsAddedEmailAsync(User user, List<Artist> artists);

        Task SendTracksAddedToPlaylistEmailAsync(User user, Playlist playlist, List<Song> songs, User targetUser);
    }
}
