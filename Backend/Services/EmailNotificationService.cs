using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using MailKit;
public class EmailNotificationService : IEmailNotificationService
{
    private readonly SmtpSettings _settings;

    public EmailNotificationService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password),
            EnableSsl = true
        };

        var mailMessage = new MailMessage(_settings.FromEmail, toEmail, subject, body);
        await client.SendMailAsync(mailMessage);
    }
}
