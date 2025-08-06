using Notification.Domain.Models;

namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendOtpAsync(UserResponseDto user, string templateFile);
        Task<bool> SendTransactionNotificationAsync(TransactionPayload payload, UserResponseDto user, string templateFile);
        Task<bool> SendOtpToUser(UserResponseDto user, string method, string templateFile);
        Task<bool> SendTransactionNotification(TransactionPayload payload, UserResponseDto user, string method, string templateFile);
        bool ValidateOtp(string userIdentifier, string inputCode);
        Task<bool> SendKycLimitAlert(UserResponseDto user, string templateFile);
    }
}
