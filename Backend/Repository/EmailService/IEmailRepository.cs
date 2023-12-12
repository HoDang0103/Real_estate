using Microsoft.AspNetCore.Mvc;

namespace Backend.Repository.EmailService
{
    public interface IEmailRepository
    {
        public Task<string> SendPasswordResetEmailAsync(string userEmail);
        public Task<string> SendOTPEmailAsync(string userEmail);
        public Task<string> VerifyOTPAndResetPasswordAsync(string userEmail, string enteredOTP, string newPassword);

    }
}
