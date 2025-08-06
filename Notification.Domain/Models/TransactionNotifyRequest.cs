namespace Notification.Domain.Models
{
    public class TransactionNotifyRequest
    {
        public UserResponseDto User {  get; set; }
        
        public TransactionPayload payload { get; set; }
        public string Method { get; set; } // "sms", "email", or "both"
        public string Message { get; set; }
    }
    
}
