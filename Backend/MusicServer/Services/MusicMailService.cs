using DataAccess.Entities;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
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

        public Task SendPlaylistAddedFromUserEmail(User user, Playlist playlist, User addedUser)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaylistRemovedFromUserEmail(User user, Playlist playlist, User removedUser)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaylistSharedWithUserEmail(User user, Playlist playlist, User sharedUser)
        {
            throw new NotImplementedException();
        }

        public Task SendTracksAddedFromArtistEmail(User user, Artist artist, List<Song> songs)
        {
            throw new NotImplementedException();
        }

        public Task SendTracksAddedToPlaylistEmail(User user, Playlist playlist, List<Song> songs, User targetUser)
        {
            throw new NotImplementedException();
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
