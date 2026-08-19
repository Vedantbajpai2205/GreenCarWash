public interface IEmailNotificationService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}