namespace Notification.Domain.Models
{
    public class SMSRequest
    {
        public string ToPhoneNumber { get; set; }
        public string Message { get; set; }
    }
}
