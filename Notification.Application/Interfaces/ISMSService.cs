using Notification.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Application.Interfaces
{
    public interface ISMSService
    {
        Task<bool> SendSms(SMSRequest smsRequest);
    }
}
