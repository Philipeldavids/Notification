using Notification.Domain.Models;

namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendOtpAsync(string toEmail);
        Task<bool> SendTransactionNotificationAsync(string toEmail, string message);
        Task<bool> SendOtpToUser(MailRequest mailRequest, SMSRequest smsRequest, string method);
        Task<bool> SendTransactionNotification(MailRequest mailRequest, SMSRequest smsRequest, string method, string message);
    }
}
