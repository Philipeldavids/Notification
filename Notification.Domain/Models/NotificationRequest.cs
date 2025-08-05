using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Models
{
    public class NotificationRequest
    {
        public string Method { get; set; } // "sms", "email", "both"
        public MailRequest Mail { get; set; }
        public SMSRequest Sms { get; set; }
    }
}
