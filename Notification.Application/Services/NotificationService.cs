using Notification.Application.Interfaces;
using Notification.Application.UtilityHelpers;
using Notification.Domain.Models;

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

        public async Task<bool> SendOtpAsync(string toEmail)
        {
            var otp = OtpGenerator.GenerateNumericOtp();

            var mailRequest = new MailRequest
            {
                ToEmail = toEmail,
                Subject = "VERIFICATION",
                Body = $"Your OTP is: {otp}"
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }

        public async Task<bool> SendOtpToUser(MailRequest mailRequest, SMSRequest smsRequest, string method)
        {
            var otp = OtpGenerator.GenerateNumericOtp();
            var message = $"Your OTP code is: {otp}";

            mailRequest.Subject = "Your OTP Code";
            mailRequest.Body = message;
            smsRequest.Message = message;

            return await DispatchNotification(method, mailRequest, smsRequest);
        }

        public async Task<bool> SendTransactionNotificationAsync(string toEmail, string message)
        {
            var mailRequest = new MailRequest
            {
                ToEmail = toEmail,
                Subject = "Transaction Alert",
                Body = message
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }

        public async Task<bool> SendTransactionNotification(MailRequest mailRequest, SMSRequest smsRequest, string method, string message)
        {
            mailRequest.Subject = "Transaction Alert";
            mailRequest.Body = message;
            smsRequest.Message = message;

            return await DispatchNotification(method, mailRequest, smsRequest);
        }

        private async Task<bool> DispatchNotification(string method, MailRequest mailRequest, SMSRequest smsRequest)
        {
            bool emailSent = false, smsSent = false;

            try
            {
                switch (method.ToLower())
                {
                    case "email":
                        emailSent = await _emailService.SendEmailAsync(mailRequest);
                        break;

                    case "sms":
                        smsSent = await _smsService.SendSmsAsync(smsRequest);
                        break;

                    case "both":
                        emailSent = await _emailService.SendEmailAsync(mailRequest);
                        smsSent = await _smsService.SendSmsAsync(smsRequest);
                        break;

                    default:
                        throw new ArgumentException("Invalid notification method. Use 'email', 'sms', or 'both'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification failed: {ex.Message}");
            }
            if (!emailSent && !smsSent)
                throw new Exception("Both email and SMS failed to send.");
            return true;
        }
    }
}
