using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Notification.Application.Interfaces;
using Notification.Domain.Models;
using Microsoft.Extensions.Configuration;

namespace Notification.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly string smtpHost; // e.g., smtp.gmail.com
        private readonly int smtpPort;
        private readonly string smtpUser;
        private readonly string smtpPass;

        public EmailService(IConfiguration config)
        {
            smtpHost = config["SMTPMAILSETTINGS:SMTPHOST"];
            smtpPort = int.Parse(config["SMTPMAILSETTINGS:SMTPPORT"]);
            smtpUser = config["SMTPMAILSETTINGS:SMTPUSER"];
            smtpPass = config["SMTPMAILSETTINGS:SMTPPASS"];
        }
        public async Task<bool> SendEmail(MailRequest mailRequest)
        {
            var message = new MailMessage(smtpUser, mailRequest.ToEmail, mailRequest.Subject, mailRequest.Body)
            {
                IsBodyHtml = true
            };

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
            return true;
        }
    }
}
