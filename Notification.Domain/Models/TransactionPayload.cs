using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.Domain.Models
{
    public class TransactionPayload
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Date {  get; set; }
        public double Balance { get; set; }
        public string TransactionType { get; set; }
    }
}
