using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Models
{
    public class SMSRequest
    {
        public string ToPhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
