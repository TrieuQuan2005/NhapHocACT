using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace hehehe.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string to, string subject, string body, string[] attachments)
        {
            var emailSettings = _config.GetSection("EmailSettings");
            var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]),
                Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"], emailSettings["SenderName"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            if (attachments != null)
            {
                foreach (var file in attachments)
                {
                    if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
                    {
                        mailMessage.Attachments.Add(new Attachment(file));
                    }
                }
            }

            smtpClient.Send(mailMessage);
        }

    }
}