using Backend.Repository.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;

        public EmailController(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        [Authorize]
        [HttpPost("RefreshPassword")]
        public async Task<IActionResult> SendEmail(string userEmail)
        {
            var result = await _emailRepository.SendPasswordResetEmailAsync(userEmail);

            if (result == "Succeed")
            {
                return Ok("Password refreshed successfully. Check your email for the new password.");
            }
            if (result == "Fail")
            {
                return BadRequest("Email not found.");
            }
            return BadRequest("Failed to send the email.");
        }


        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(string userEmail)
        {
            var result = await _emailRepository.SendOTPEmailAsync(userEmail);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Email not found.");
            }
            return Ok("OTP sent successfully. Please check your email or phone.");
        }


        [HttpPost("VerifyOTPAndResetPassword")]
        public async Task<IActionResult> VerifyOTPAndResetPassword(string userEmail, string enteredOTP, string newPassword)
        {
            var result = await _emailRepository.VerifyOTPAndResetPasswordAsync(userEmail, enteredOTP, newPassword);
            if (result == "NotFount")
            {
                return BadRequest("Email not found.");
            }
            if (result == "OTPFail")
            {
                return BadRequest("Invalid OTP. Please try again.");
            }
            if (result == "Success")
            {
                return Ok("Password reset successful.");
            }
            return BadRequest("Failed to reset the password. Please try again.");
        }
    }
}
