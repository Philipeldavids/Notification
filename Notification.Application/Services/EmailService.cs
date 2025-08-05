using System.Net.Mail;
using System.Net;
using Notification.Application.Interfaces;
using Notification.Domain.Models;
using Notification.Application.Options;
using Microsoft.Extensions.Options;

namespace Notification.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpOptions)
        {
            _smtpSettings = smtpOptions.Value;
        }

        public async Task<bool> SendEmailAsync(MailRequest request)
        {
            using var client = new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = true,
                Timeout=20000,
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username, "SwissPay"),
                Subject = request.Subject,
                Body = request.Body,
                // IsBodyHtml = false
            };

            mail.To.Add(request.ToEmail);
            mail.ReplyToList.Add("noreply@swisspay.com");

            try
            {
                await client.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }

}