using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ToDo.API.Services.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            // Read SMTP settings
            var host = _config["Email:SmtpHost"];
            var port = int.Parse(_config["Email:SmtpPort"] ?? "465");
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var useSsl = bool.TryParse(_config["Email:UseSsl"], out var ssl) ? ssl : true;
            var useStartTls = bool.TryParse(_config["Email:UseStartTls"], out var st) ? st : false;

            using var client = new SmtpClient();
            try
            {
                // Connect: either SSL port (465) or StartTls (587)
                if (useStartTls)
                {
                    await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                }
                else
                {
                    await client.ConnectAsync(host, port, useSsl ? MailKit.Security.SecureSocketOptions.SslOnConnect : MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
                }

                if (!string.IsNullOrEmpty(username))
                    await client.AuthenticateAsync(username, password);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                _logger.LogInformation("Email sent to {Email}. Subject: {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed sending email to {Email}", toEmail);
                throw; // rethrow so caller can handle (or remove to swallow)
            }
        }
    }
}
