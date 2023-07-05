using DataAccess.Entities;

namespace MusicServer.Interfaces
{
    public interface IMusicMailService
    {
        Task SendWelcomeEmail(User user, string activationlink);

        Task SendPasswordResetEmail(User user, string resetlink);

        Task SendEmailChangeEmail(User user, string changeMailLink);

        Task SendPlaylistAddedFromUserEmail(User user, Playlist playlist, User addedUser);

        Task SendPlaylistSharedWithUserEmail(User user, Playlist playlist, User sharedUser);

        Task SendPlaylistRemovedFromUserEmail(User user, Playlist playlist, User removedUser);

        Task SendTracksAddedFromArtistEmail(User user, Artist artist, List<Song> songs);

        Task SendTracksAddedToPlaylistEmail(User user, Playlist playlist, List<Song> songs, User targetUser);
    }
}
