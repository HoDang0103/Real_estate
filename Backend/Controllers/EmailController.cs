using Backend.Models;
using Backend.Repository.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _emailRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailController(IEmailRepository emailRepository, UserManager<ApplicationUser> userManager)
        {
            _emailRepository = emailRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost("RefreshPassword")]
        public async Task<IActionResult> SendEmail()
        {
            try
            {
                // Lấy email của người dùng đang đăng nhập
                var user = await _userManager.GetUserAsync(User);

                if (user != null)
                {
                    var userEmail = await _userManager.GetEmailAsync(user);

                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        // Gửi email đặt lại mật khẩu
                        var result = await _emailRepository.SendPasswordResetEmailAsync(userEmail);

                        if (result == "Succeed")
                        {
                            return Ok("Password refreshed successfully. Check your email for the new password.");
                        }
                        else
                        {
                            return BadRequest("Failed to send the email.");
                        }
                    }
                    else
                    {
                        return BadRequest("Email not found.");
                    }
                }
                else
                {
                    return BadRequest("User not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
