using Notification.Domain.Models;

namespace Notification.Application.Interfaces
{
    public interface ISMSService
    {
        Task<bool> SendSmsAsync(SMSRequest smsRequest);
    }
}
