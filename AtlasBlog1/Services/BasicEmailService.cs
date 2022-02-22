using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace AtlasBlog1.Services
{
    public class BasicEmailService : IEmailSender
    {
        private readonly IConfiguration _config;

        public BasicEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //We need to compose an Email based partially on the data supplied by the user
            var emailSender = _config["SmtpSettings:UserName"];

            MimeMessage newEmail = new();

            newEmail.Sender = MailboxAddress.Parse(emailSender);
            newEmail.To.Add(MailboxAddress.Parse(email));
            newEmail.Subject = subject;

            //The body of the email is a bit more tricky
            BodyBuilder emailBody = new();
            emailBody.HtmlBody = htmlMessage;
            newEmail.Body = emailBody.ToMessageBody();

            //At this point we are done composing the email and now we need to turn
            //our focus on configuring the Simple Mail Transfer Protocol (SMTP) server
            using SmtpClient smtpClient = new();

            var host = _config["SmtpSettings:Host"];
            var port = Convert.ToInt32(_config["SmtpSettings:Port"]);

            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(emailSender, _config["SmtpSettings:Password"]);
            await smtpClient.SendAsync(newEmail);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
