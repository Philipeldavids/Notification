using Notification.Application.Interfaces;
using Notification.Application.UtilityHelpers;
using Notification.Domain.Models;
using System.Globalization;

namespace Notification.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        private static Dictionary<string, OtpToken> otpStore = new(); // key: user/email/phone


        public NotificationService(IEmailService emailService, ISMSService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task<bool> SendOtpAsync(UserResponseDto user, string templateFile)
        {
            var otp = OtpGenerator.GenerateNumericOtp();
            var template = GetTemplate.GetEmailTemplate(templateFile);
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.FirstName);

            template = template.Replace("{UserName}", $"{userName}");
            template = template.Replace("{OTP}", $"{otp}");

            OtpToken otpToken = new()
            {
                Code = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            otpStore[user.Email] = otpToken;

            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "PHONE VERIFICATION",
                Body = template,
                IsHtml = true,
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }

        public async Task<bool> SendOtpToUser(UserResponseDto user, string method, string templateFile)
        {
            var otp = OtpGenerator.GenerateNumericOtp();
            var template = GetTemplate.GetEmailTemplate(templateFile);
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.FirstName);

            OtpToken otpToken = new()
            {
                Code = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            otpStore[user.Email] = otpToken;

            MailRequest mailRequest = new();
           
            mailRequest.ToEmail = user.Email;
            mailRequest.Subject = "Your OTP Code";
            mailRequest.Body = template;
            mailRequest.IsHtml = true;

            SMSRequest smsRequest = new();

            var message = $"Your OTP code is: {otp}. It expires in 5 minutes.";
            smsRequest.Message = message;

            return await DispatchNotification(method, mailRequest, smsRequest);
        }

        public async Task<bool> SendKycLimitAlert(UserResponseDto user, string templateFile)
        {
            var template = GetTemplate.GetEmailTemplate(templateFile);
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.FirstName);

            template = template.Replace("{UserName}", $"{userName}");

            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Transaction Alert",
                Body = template,
                IsHtml = true
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }
        public async Task<bool> SendTransactionNotificationAsync(TransactionPayload payload, UserResponseDto user, string templateFile)
        {
            var template = GetTemplate.GetEmailTemplate(templateFile);
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.FirstName);

            template = template.Replace("{UserName}", $"{userName}");
            template = template.Replace("{TransactionType}", $"{payload.TransactionType}");
            template = template.Replace("{Amount}", $"{payload.Amount}");
            template = template.Replace("{Description}", $"{payload.Description}");
            template = template.Replace("{Date}", $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")}");
            template = template.Replace("{Balance}", $"{payload.Balance}");

            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Transaction Alert",
                Body = template,
                IsHtml= true
            };

            return await _emailService.SendEmailAsync(mailRequest);
        }

        public async Task<bool> SendTransactionNotification(TransactionPayload payload, UserResponseDto user, string method, string templateFile)
        {
            var template = GetTemplate.GetEmailTemplate(templateFile);
            TextInfo textInfo = new CultureInfo("en-GB", false).TextInfo;
            var userName = textInfo.ToTitleCase(user.FirstName);

            template = template.Replace("{UserName}", $"{userName}");
            template = template.Replace("{TransactionType}", $"{payload.TransactionType}");
            template = template.Replace("{Amount}", $"{payload.Amount}");
            template = template.Replace("{Description}", $"{payload.Description}");
            template = template.Replace("{Date}", $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")}");
            template = template.Replace("{Balance}", $"{payload.Balance}");

            MailRequest mailRequest = new();

            mailRequest.ToEmail = user.Email;
            mailRequest.Subject = "Transaction Alert";
            mailRequest.Body = template;
            mailRequest.IsHtml = true;

            SMSRequest smsRequest = new();
            smsRequest.Message = $"{payload.TransactionType} {payload.Amount} {payload.Description} {payload.Date} {payload.Balance}";
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

        public bool ValidateOtp(string userIdentifier, string inputCode)
        {
            if (!otpStore.TryGetValue(userIdentifier, out var otp)) return false;
            return otp.Code == inputCode && otp.ExpiresAt > DateTime.UtcNow;
        }

    }

}
