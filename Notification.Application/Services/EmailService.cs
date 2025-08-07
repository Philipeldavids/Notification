//using System.Net.Mail;
using System.Net;
using Notification.Application.Interfaces;
using Notification.Domain.Models;
using Notification.Application.Options;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

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

            //{
            //    Host = _smtpSettings.Host,
            //    Port = _smtpSettings.Port,
            //    EnableSsl = true,
            //    Timeout=20000,
            //    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password)
            //};

            using var mail = new MimeMessage()
            {
                Sender = MailboxAddress.Parse(_smtpSettings.Username),
                Subject = request.Subject,
               
            };
            //var mail = new MailMessage
            //{
            //    From = new MailAddress(_smtpSettings.Username, "SwissPay"),
            //    Subject = request.Subject,
            //    Body = request.Body,
            //    // IsBodyHtml = false
            //};
            var builder = new BodyBuilder();
            builder.HtmlBody = request.Body;

            mail.Body = builder.ToMessageBody();
            mail.To.Add(MailboxAddress.Parse(request.ToEmail));
            //mail.ReplyToList.Add("noreply@swisspay.com");

            try
            {
                using var client = new SmtpClient();
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.SslOnConnect);
                client.Authenticate(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(mail);
                client.Disconnect(true);                
                return true;
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task SendBulkEmailsAsync(BulkMailRequest bulkMailRequest)
        {
            foreach (var email in bulkMailRequest.Emails)
            {
                MailRequest request = new()
                {
                    ToEmail = email,
                    Subject = bulkMailRequest.Subject,
                    Body = bulkMailRequest.Message
                };
                
                await SendEmailAsync(request);
            }
        }

    }


}