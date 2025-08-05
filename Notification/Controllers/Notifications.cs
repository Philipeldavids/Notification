using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.Interfaces;
using Notification.Application.Services;
using Notification.Domain.Models;

namespace Notification.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Notifications : ControllerBase
    {
        private readonly INotificationService _notification;

        public Notifications(INotificationService notification)
        {
            _notification = notification;
        }
        [HttpPost("SendOTPtoUser")]
        public async Task<IActionResult> SendNotification(
           [FromBody] NotificationRequest request
        )
        {
            try
            {
                var result = await _notification.SendOtpToUser(request.Mail, request.Sms, request.Method.ToLower());
                if (result)
                {
                    return Ok(new { message = "OTP sent successfully." });
                }
                return BadRequest(new { error = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("SendTransactionNotification")]
        public async Task<IActionResult> SendTransactionNotification([FromBody] TransactionNotifyRequest request)
        {
            try
            {
                var result = await _notification.SendTransactionNotification(request.Mail, request.Sms, request.Method, request.Message);
                if (result)
                {
                    return Ok(new { message = "Notification sent." });
                }
                return BadRequest(new { error = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

    }
}
