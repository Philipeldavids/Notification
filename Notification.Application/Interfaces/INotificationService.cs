using Notification.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface INotificationService
    {
        Task<bool> SendTransactionNotification(MailRequest mailRequest, SMSRequest sMSRequest, string method, string message);
        Task<bool> SendOtpToUser(MailRequest mailRequest, SMSRequest sMSRequest, string method);
    }
}
