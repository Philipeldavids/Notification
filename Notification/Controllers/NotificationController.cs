using Microsoft.AspNetCore.Mvc;
using Notification.Application.Interfaces;
using Notification.Domain.Models;

namespace Notification.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Send OTP via email
        /// </summary>
        /// <param name="request">The email address to send OTP to</param>
        /// <returns></returns>
        [HttpPost("otp")]
        public async Task<IActionResult> SendOtpAsync([FromBody] OtpEmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
                return BadRequest(new { error = "Email address is required." });

            try
            {
                var result = await _notificationService.SendOtpAsync(request.Email);
                if (!result)
                    return StatusCode(500, new { error = "Failed to send OTP." });

                return Ok(new { message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Send transaction alert via email, SMS or both
        /// </summary>
        [HttpPost("transaction")]
        public async Task<IActionResult> SendTransactionNotificationAsync([FromBody] TransactionNotifyRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Request body is missing." });

            if (string.IsNullOrWhiteSpace(request.Method))
                return BadRequest(new { error = "Notification method is required (email, sms, both)." });

            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest(new { error = "Message is required." });

            try
            {
                var result = await _notificationService.SendTransactionNotification(
                    request.Mail,
                    request.Sms,
                    request.Method,
                    request.Message
                );

                if (!result)
                    return StatusCode(500, new { error = "Failed to send transaction notification." });

                return Ok(new { message = "Transaction notification sent successfully." });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { error = argEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
