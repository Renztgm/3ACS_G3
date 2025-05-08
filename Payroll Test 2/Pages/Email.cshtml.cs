using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Payroll_Test_2.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string content);
        Task<bool> SendPayrollNotificationAsync(string to, string content);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromDisplayName;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load email configuration from appsettings.json
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = _configuration["EmailSettings:Username"];
            _smtpPassword = _configuration["EmailSettings:Password"];
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromDisplayName = _configuration["EmailSettings:FromDisplayName"];
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string content)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromDisplayName),
                    Subject = subject,
                    Body = content,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    await client.SendMailAsync(message);
                }

                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {to}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPayrollNotificationAsync(string to, string content)
        {
            const string subject = "Payroll Notification";
            return await SendEmailAsync(to, subject, content);
        }
    }
}