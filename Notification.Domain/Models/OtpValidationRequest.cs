using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Models
{
    public class OtpValidationRequest
    {
        public string UserIdentifier { get; set; } // Email or phone
        public string Code { get; set; }
    }

}
