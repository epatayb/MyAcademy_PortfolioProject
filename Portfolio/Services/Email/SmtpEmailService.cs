using System.Net;
using System.Net.Mail;

namespace Portfolio.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(
            string recipientEmail,
            string subject,
            string htmlBody)
        {
            var host = _configuration["Smtp:Host"];
            var userName = _configuration["Smtp:UserName"];
            var password = _configuration["Smtp:Password"];
            var fromEmail = _configuration["Smtp:FromEmail"];
            var fromName = _configuration["Smtp:FromName"];

            var portText = _configuration["Smtp:Port"];

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException(
                    "Smtp:Host ayarı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidOperationException(
                    "Smtp:UserName ayarı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Smtp:Password ayarı bulunamadı.");
            }

            if (string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new InvalidOperationException(
                    "Smtp:FromEmail ayarı bulunamadı.");
            }

            if (!int.TryParse(portText, out var port))
            {
                throw new InvalidOperationException(
                    "Smtp:Port ayarı geçersiz.");
            }

            password = password.Replace(" ", string.Empty);

            using var message = new MailMessage
            {
                From = new MailAddress(
                    fromEmail,
                    fromName ?? "Portfolio Admin"),

                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(recipientEmail);

            using var smtpClient = new SmtpClient(host, port)
            {
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(
                    userName,
                    password),

                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            await smtpClient.SendMailAsync(message);
        }
    }
}