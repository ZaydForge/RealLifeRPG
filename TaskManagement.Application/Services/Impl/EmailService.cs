using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Services.Impl;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _config;

    public EmailService(IOptions<EmailConfiguration> config)
    {
        _config = config.Value;
    }
    public async Task<bool> SendOtpAsync(string toEmail, string otp)
    {
        try
        {
            using var client = new SmtpClient(_config.SmtpServer, _config.Port)
            {
                EnableSsl = _config.EnableSsl,
                Credentials = new NetworkCredential(_config.Username, _config.Password)
            };

            var message = new MailMessage
            {
                From = new MailAddress(_config.DefaultFromEmail, _config.DefaultFromName),
                Subject = "Real Life RPG: OTP Verification Code",
                Body = GenerateBody(otp),
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
            return true;
        }
        catch (SmtpException smtpEx)
        {
            // SMTP ga oid xatolarni qayd qiling
            Console.WriteLine($"SMTP error while sending email to '{toEmail}': {smtpEx.StatusCode} - {smtpEx.Message}");
            if (smtpEx.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {smtpEx.InnerException.Message}");
            }
            return false;
        }
        catch (Exception ex)
        {
            // Umumiy istisnolarni qayd qiling
            Console.WriteLine($"Error while sending email to '{toEmail}': {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return false;
        }
    }

    private string GenerateBody(string otp)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<html><body style='font-family:sans-serif;'>");
        sb.AppendLine("<h3>Welcome to Real Life RPG!</h3>");
        sb.AppendLine("<p>Your one-time verification code is:</p>");
        sb.AppendLine($"<div style='font-size: 24px; font-weight: bold; margin: 20px 0;'>{otp}</div>");
        sb.AppendLine("<p>Please do not share this code with anyone. It will expire in 10 minutes.</p>");
        sb.AppendLine("<p>If you did not request this, please ignore.</p>");
        sb.AppendLine("<br/><small>&copy; 2025 Real Life RPG</small>");
        sb.AppendLine("</body></html>");
        return sb.ToString();
    }
}
