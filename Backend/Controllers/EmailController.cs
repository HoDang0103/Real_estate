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
                            return Ok( new { message = "Password refreshed successfully. Check your email for the new password." });
                        }
                        else
                        {
                            return BadRequest( new { message = "Failed to send the email." });
                        }
                    }
                    else
                    {
                        return BadRequest( new { message = "Email not found." });
                    }
                }
                else
                {
                    return BadRequest( new { message = "User not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }


        [HttpPost("SendOTP")]
        public async Task<IActionResult> SendOTP(string userEmail)
        {
            var result = await _emailRepository.SendOTPEmailAsync(userEmail);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(new { message = "Email not found." });
            }
            return Ok( new { message = "OTP sent successfully. Please check your email or phone." });
        }


        [HttpPost("VerifyOTPAndResetPassword")]
        public async Task<IActionResult> VerifyOTPAndResetPassword(string userEmail, string enteredOTP, string newPassword)
        {
            var result = await _emailRepository.VerifyOTPAndResetPasswordAsync(userEmail, enteredOTP, newPassword);
            if (result == "NotFount")
            {
                return BadRequest( new { message = "Email not found." });
            }
            if (result == "OTPFail")
            {
                return BadRequest( new { message = "Invalid OTP. Please try again." });
            }
            if (result == "Success")
            {
                return Ok( new { message = "Password reset successful." });
            }
            return BadRequest( new { message = "Failed to reset the password. Please try again." });
        }
    }
}
