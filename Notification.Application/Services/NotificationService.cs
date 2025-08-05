using Notification.Application.Interfaces;
using Notification.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;

        public NotificationService(IEmailService emailService, ISMSService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }
        public async Task<bool> SendOtpToUser(MailRequest mailRequest, SMSRequest sMSRequest, string method)
        {
            var otp = OtpGenerator.GenerateNumericOtp();
            var message = $"Your OTP code is: {otp}";

            mailRequest.Subject = "Your OTP Code";
            mailRequest.Body = message;
            sMSRequest.Message = message; 

            switch (method)
            {
                case "email":
                   return await _emailService.SendEmail(mailRequest);                    

                case "sms":
                    return await _smsService.SendSms(sMSRequest);
                    

                case "both":
                    await _emailService.SendEmail(mailRequest);
                    return await _smsService.SendSms(sMSRequest);
                    

                default:
                    throw new ArgumentException("Invalid notification method. Use 'sms', 'email', or 'both'.");
            }

            // Optional: Save OTP with expiration in DB here
        }

        public async Task<bool> SendTransactionNotification(MailRequest mailRequest, SMSRequest sMSRequest, string method, string message)
        {
            bool emailSuccess = false, smsSuccess = false;
            string emailError = null, smsError = null;

            if (method == "email" || method == "both")
            {
                mailRequest.Subject = "Transaction Alert";
                mailRequest.Body = message;
                try
                {
                    await _emailService.SendEmail(mailRequest);
                    emailSuccess = true;
                }
                catch (Exception ex)
                {
                    emailError = ex.Message;
                }
            }

            if (method == "sms" || method == "both")
            {
                sMSRequest.Message = message;
                try
                {
                    await _smsService.SendSms(sMSRequest);
                    smsSuccess = true;
                }
                catch (Exception ex)
                {
                    smsError = ex.Message;
                }
            }

            // Save log(s)
            //if (method == "email" || method == "both")
            //    await SaveTransactionNotification(email, message, "email", emailSuccess, emailError);

            //if (method == "sms" || method == "both")
            //    await SaveTransactionNotification(phone, message, "sms", smsSuccess, smsError);

            if (!emailSuccess && !smsSuccess)
                throw new Exception("Both SMS and Email failed.");
            return true;
        }

    }


    public class OtpGenerator
    {
        public static string GenerateNumericOtp(int length = 6)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return string.Concat(bytes.Select(b => (b % 10).ToString()));
        }
    }
}
