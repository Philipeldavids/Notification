using Notification.Domain.Models;

namespace Notification.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(MailRequest request);
    }

}
