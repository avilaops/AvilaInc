namespace Manager.Api.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendWelcomeEmailAsync(string to, string name, string verifyToken);
    Task SendPasswordResetEmailAsync(string to, string name, string resetToken);
}