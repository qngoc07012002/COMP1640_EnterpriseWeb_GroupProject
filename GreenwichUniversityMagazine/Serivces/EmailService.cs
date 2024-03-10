using GreenwichUniversityMagazine.Serivces.IServices;
using Microsoft.Extensions.Options;
using Sending_Mail.Models;
using System.Net.Mail;
using System.Net;

namespace GreenwichUniversityMagazine.Serivces
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = _emailSettings.Username,
                    Password = _emailSettings.Password
                };

                client.Credentials = credential;
                client.Host = _emailSettings.MailServer;
                client.Port = _emailSettings.MailPort;
                client.EnableSsl = _emailSettings.EnableSSL;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName);
                    emailMessage.To.Add(email);
                    emailMessage.Subject = subject;
                    emailMessage.Body = message;
                    emailMessage.IsBodyHtml = true;
                    await client.SendMailAsync(emailMessage);
                }
            }
        }
    }
}
