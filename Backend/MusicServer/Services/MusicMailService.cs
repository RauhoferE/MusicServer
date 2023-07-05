using DataAccess.Entities;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MusicServer.Const;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using MusicServer.Settings;
using Serilog;

namespace MusicServer.Services
{
    public class MusicMailService : IMusicMailService
    {
        private readonly MailSettings _mailSettings;

        public MusicMailService(IOptions<MailSettings> mailSettings)
        {
            this._mailSettings = mailSettings.Value;
        }

        public async Task SendEmailChangeEmail(User user, string changeMailLink)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.TemporarayEmail));
            message.Subject = "Password Reset received";
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/EmailResetEmail.html").Replace("{activationlink}", changeMailLink),
            };

            await this.SendMessage(message);
        }

        public async Task SendNewArtistsAddedEmail(User user, List<Artist> artists)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.TemporarayEmail));
            message.Subject = "New Artists on Project Siren";

            var htmlText = File.ReadAllText("Assets/EmailTemplates/ArtistsAddedEmail.html");
            var artistsAnchors = string.Empty;

            foreach (var artist in artists)
            {
                //TODO: Change to frontend address
                artistsAnchors = artistsAnchors + $"<a href=\"https://localhost:7001/{ApiRoutes.Song.Artist.Replace("{artistId}", artist.Id.ToString())}\">{artist.Name}</a><br>";
            }

            htmlText = htmlText.Replace("{artists}", artistsAnchors);

            message.Body = new TextPart("html")
            {
                Text = htmlText,
            };

            await this.SendMessage(message);
        }

        public async Task SendPasswordResetEmail(User user, string resetlink)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Change Email Request received";
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/PasswordResetEmail.html").Replace("{activationlink}", resetlink),
            };

            await this.SendMessage(message);
        }

        public async Task SendPlaylistAddedFromUserEmail(User user, Playlist playlist, User addedUser)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(addedUser.UserName, addedUser.Email));
            message.Subject = "New Playlist for you";
            //TODO: Change to frontend address
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/PlaylistAddedFromUserEmail.html")
                .Replace("{user}", user.UserName)
                .Replace("{playlistname}", playlist.Name)
                .Replace("{playlistlink}", $"https://localhost:7001/{ApiRoutes.Playlist.Songs.Replace("{playlistId}", playlist.Id.ToString())}"),
            };

            await this.SendMessage(message);
        }

        public async Task SendPlaylistRemovedFromUserEmail(User user, Playlist playlist, User removedUser)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(removedUser.UserName, removedUser.Email));
            message.Subject = "You were removed from a playlist";
            //TODO: Change to frontend address
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/UserPlaylistRemoveUserEmail.html")
                .Replace("{user}", user.UserName)
                .Replace("{playlist}", playlist.Name)
            };

            await this.SendMessage(message);
        }

        public async Task SendPlaylistSharedWithUserEmail(User user, Playlist playlist, User sharedUser)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(sharedUser.UserName, sharedUser.Email));
            message.Subject = "New Playlist for you";
            //TODO: Change to frontend address
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/UserPlaylistAddUserEmail.html")
                .Replace("{user}", user.UserName)
                .Replace("{playlistname}", playlist.Name)
                .Replace("{playlistlink}", $"https://localhost:7001/{ApiRoutes.Playlist.Songs.Replace("{playlistId}", playlist.Id.ToString())}"),
            };

            await this.SendMessage(message);
        }

        public async Task SendTracksAddedFromArtistEmail(User user, Artist artist, List<Song> songs)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.TemporarayEmail));
            message.Subject = "New Releases for your followed artist";

            var htmlText = File.ReadAllText("Assets/EmailTemplates/TracksAddedFromArtistEmail.html")
                .Replace("{artist}", $"{artist.Name}");
            var songsAnchor = string.Empty;

            foreach (var song in songs)
            {
                //TODO: Change to frontend address
                songsAnchor = songsAnchor + $"<a href=\"https://localhost:7001/{ApiRoutes.Song.SongDefault.Replace("{songId}", song.Id.ToString())}\">{song.Name}</a><br>";
            }

            htmlText = htmlText.Replace("{newTracks}", songsAnchor);

            message.Body = new TextPart("html")
            {
                Text = htmlText,
            };

            await this.SendMessage(message);
        }

        public async Task SendTracksAddedToPlaylistEmail(User user, Playlist playlist, List<Song> songs, User targetUser)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(targetUser.UserName, targetUser.TemporarayEmail));
            message.Subject = "New Songs added to a playlist you follow";

            var htmlText = File.ReadAllText("Assets/EmailTemplates/TracksAddedToPlaylistEmail.html")
                .Replace("{user}", user.UserName)
                .Replace("{playlistname}", playlist.Name)
                .Replace("{playlistlink}", $"https://localhost:7001/{ApiRoutes.Playlist.Default}{playlist.Id}");
            var songsAnchor = string.Empty;

            foreach (var song in songs)
            {
                //TODO: Change to frontend address
                songsAnchor = songsAnchor + $"<a href=\"https://localhost:7001/{ApiRoutes.Song.SongDefault.Replace("{songId}", song.Id.ToString())}\">{song.Name}</a><br>";
            }

            htmlText = htmlText.Replace("{tracksAdded}", songsAnchor);

            message.Body = new TextPart("html")
            {
                Text = htmlText,
            };

            await this.SendMessage(message);
        }

        public async Task SendWelcomeEmail(User user, string activationlink)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = "Welcome to Project Siren";
            Log.Information(File.ReadAllText("Assets/EmailTemplates/WelcomeConfirmEmail.html")
                .Replace("{activationlink}", activationlink).Replace("{username}", user.UserName));
            message.Body = new TextPart("html")
            {
                Text = File.ReadAllText("Assets/EmailTemplates/WelcomeConfirmEmail.html")
                .Replace("{activationlink}", activationlink).Replace("{username}", user.UserName),
            };

            await this.SendMessage(message);
        }

        private async Task SendMessage(MimeMessage message)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(this._mailSettings.Host, this._mailSettings.Port);
                    client.Authenticate(this._mailSettings.Email, this._mailSettings.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception e)
            {
                throw new MailSendException("Mail couldn't be sent.", e);
            }
        }
    }
}
