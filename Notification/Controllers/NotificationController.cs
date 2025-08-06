using Azure.Core;
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
        public async Task<IActionResult> SendOtpAsync([FromBody] UserResponseDto user)
        {
            if (string.IsNullOrWhiteSpace(user?.Email))
                return BadRequest(new { error = "Email address is required." });

            try
            {
                var result = await _notificationService.SendOtpAsync(user, "otp_verification_template.html");
                if (!result)
                    return StatusCode(500, new { error = "Failed to send OTP." });

                return Ok(new { message = "OTP sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("validate-otp")]
        public IActionResult ValidateOtp([FromBody] OtpValidationRequest request)
        {
            var valid = _notificationService.ValidateOtp(request.UserIdentifier, request.Code);
            if (!valid)
                return BadRequest("Invalid or expired OTP.");

            return Ok("OTP validated successfully.");
        }

        [HttpPost("Kyc-limit-alert")]
        public async Task<IActionResult> SendKycLimitAlert(UserResponseDto user)
        {
            if (user == null)
                return BadRequest(new { error = "User body is missing." });

            if (user.Email == null)
                return BadRequest(new { error = "Email is Required." });

            try
            {
                var result = await _notificationService.SendKycLimitAlert(
               user,
                    "kyc_limit_alert_template.html"
                );

                if (!result)
                    return StatusCode(500, new { error = "Failed to send kyc-limit notification." });

                return Ok(new { message = "Kyc-limit notification sent successfully." });
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
                    request.payload,
                    request.User,
                    request.Method,
                    "transaction_notification_template.html"
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
