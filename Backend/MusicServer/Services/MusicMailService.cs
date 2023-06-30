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
    }
}
