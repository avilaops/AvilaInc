using System.Net;
using System.Net.Mail;

namespace Manager.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        var smtpHost = smtpSettings["Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
        var smtpUser = smtpSettings["Username"];
        var smtpPass = smtpSettings["Password"];

        if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
        {
            throw new InvalidOperationException("SMTP settings not configured");
        }

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(smtpUser, "Avila Dashboard"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }

    public async Task SendWelcomeEmailAsync(string to, string name, string verifyToken)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var verifyUrl = $"{frontendUrl}/verify-email?token={verifyToken}";

        var htmlBody = $@"
            <h2>Bem-vindo ao Avila Dashboard!</h2>
            <p>Olá {name},</p>
            <p>Obrigado por se registrar. Para ativar sua conta, clique no link abaixo:</p>
            <a href=""{verifyUrl}"" style=""background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Verificar Email</a>
            <p>Este link é válido por 24 horas.</p>
            <p>Se você não se registrou, ignore este email.</p>
        ";

        await SendEmailAsync(to, "Confirmação de Conta - Avila Dashboard", htmlBody);
    }

    public async Task SendPasswordResetEmailAsync(string to, string name, string resetToken)
    {
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:3000";
        var resetUrl = $"{frontendUrl}/reset-password?token={resetToken}";

        var htmlBody = $@"
            <h2>Redefinição de Senha</h2>
            <p>Olá {name},</p>
            <p>Você solicitou a redefinição de sua senha. Clique no link abaixo para criar uma nova senha:</p>
            <a href=""{resetUrl}"" style=""background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Redefinir Senha</a>
            <p>Este link é válido por 1 hora.</p>
            <p>Se você não solicitou esta redefinição, ignore este email.</p>
        ";

        await SendEmailAsync(to, "Redefinição de Senha - Avila Dashboard", htmlBody);
    }
}