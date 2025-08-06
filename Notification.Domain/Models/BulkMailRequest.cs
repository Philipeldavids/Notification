using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Models
{
    public class BulkMailRequest
    {
        public List<string> Emails { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
