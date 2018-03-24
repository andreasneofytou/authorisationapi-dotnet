using System.Threading.Tasks;
using AuthorisationApi.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AuthorisationApi.Services
{
    public class EmailService
    {
        private readonly EmailClientOptions _emailOptions;

        public EmailService(IOptions<EmailClientOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Wedding Planner", "no-reply@andreasneofytou.eu"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailOptions.Url, _emailOptions.Port).ConfigureAwait(false);
                client.Authenticate(_emailOptions.Username, _emailOptions.Password);
                await client.SendAsync(emailMessage).ConfigureAwait(false);
                await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }
    }
}