using DataAccess.Entities;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MusicServer.Exceptions;
using MusicServer.Interfaces;
using MusicServer.Settings;

namespace MusicServer.Services
{
    public class MusicMailService : IMusicMailService
    {
        private readonly MailSettings _mailSettings;

        public MusicMailService(IOptions<MailSettings> mailSettings)
        {
            this._mailSettings = mailSettings.Value;
        }

        public async Task SendEmail(User user, string subject, string body)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(this._mailSettings.Sender, this._mailSettings.Email));
            message.To.Add(new MailboxAddress(user.UserName, user.Email));
            message.Subject = subject;
            message.Body = new TextPart("plain")
            {
                Text = body
            };

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

        public Task SendEmailChangeEmail(User user, string newMail, string changeMailLink)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetEmail(User user, string resetlink)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaylistAddedFromUserEmail(User user, Playlist playlist, User addedByUser)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaylistRemovedFromUserEmail(User user, Playlist playlist, User removedByUser)
        {
            throw new NotImplementedException();
        }

        public Task SendPlaylistSharedWithUserEmail(User user, Playlist playlist, User sharedByUser)
        {
            throw new NotImplementedException();
        }

        public Task SendTracksAddedFromArtistEmail(User user, Artist artist, List<Song> songs)
        {
            throw new NotImplementedException();
        }

        public Task SendTracksAddedToPlaylistEmail(User user, Playlist playlist, List<Song> songs, User addedByUser)
        {
            throw new NotImplementedException();
        }

        public Task SendWelcomeEmail(User user, string activationlink)
        {
            throw new NotImplementedException();
        }
    }
}
