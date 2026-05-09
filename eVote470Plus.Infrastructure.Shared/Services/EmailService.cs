using eVote470Plus.Core.Application.Dtos.Email;
using eVote470Plus.Core.Application.Interfaces.Email;
using eVote470Plus.Core.Domain.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace eVote470Plus.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailSettings> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<MailSettings> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }
        public async Task SendAsync(EmailRequestDto emailRequest)
        {
            try
            {
                emailRequest.ToRange?.Add(emailRequest.ToEmail ?? "");

                MimeMessage email = new()
                {
                    Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
                    Subject = emailRequest.Subject,
                };

                foreach(var toItem in emailRequest.ToRange ?? [])
                {
                    email.To.Add(MailboxAddress.Parse(toItem));
                }

                BodyBuilder builder = new()
                {
                    HtmlBody = emailRequest.HtmlBody
                };

                email.Body = builder.ToMessageBody();
                using MailKit.Net.Smtp.SmtpClient smtpClient = new();

                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email. {Exception}", ex);
            }

            await Task.CompletedTask;   
        }
    }
}
