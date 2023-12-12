using Backend.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Repository.EmailService
{
    public class EmailRepository : IEmailRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmailRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> SendPasswordResetEmailAsync(string userEmail)
        {
            string newPassword = GenerateRandomPassword();

            //Fill email and Password
            var senderEmail = "20520272@gm.uit.edu.vn";
            var senderName = "REAL ESTATE WEBSITE";
            var senderPassword = "";
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var enableSsl = true;

            // Create sender's email address
            var fromAddress = new MailAddress(senderEmail, senderName);
            var toAddress = new MailAddress(userEmail, "Recipient Name");

            // Create an SMTP client with your email settings
            var smtpClient = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl,
            };

            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Password Reset Instructions",
                Body = $"<p>Your new password is: {newPassword}</p>",
                IsBodyHtml = true,
            };
            try
            {
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return "Fail";
                }
                smtpClient.Send(mailMessage);


                // Update the user's password
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, newPassword);

                return "Succeed";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<string> SendOTPEmailAsync(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return string.Empty;
            }

            var otp = GenerateOTP();

            // Store the OTP in the database
            user.OTP = otp;
            await _userManager.UpdateAsync(user);

            // Send the OTP to the user (email)
            //Fill email and password
            var senderEmail = "20520272@gm.uit.edu.vn";
            var senderName = "ABC";
            var senderPassword = "";
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var enableSsl = true;

            // Create sender's email address
            var fromAddress = new MailAddress(senderEmail, senderName);
            var toAddress = new MailAddress(userEmail, "Recipient Name");

            // Create an SMTP client with your email settings
            var smtpClient = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = enableSsl,
            };
            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = "OTP Reset Instructions",
                Body = $"<p>Your OTP is: {otp}</p>",
                IsBodyHtml = true,
            };
            smtpClient.Send(mailMessage);
            return "Success";
        }

        public async Task<string> VerifyOTPAndResetPasswordAsync(string userEmail, string enteredOTP, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return "NotFound";
            }

            if (user.OTP != enteredOTP)
            {
                return "OTPFail";
            }

            // Reset the user's password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                // Clear the OTP after a successful password reset
                user.OTP = null;
                await _userManager.UpdateAsync(user);
                return "Success";
            }

            return "Failed";
        }

        private string GenerateOTP()
        {
            const string chars = "0123456789";
            var random = new Random();
            var otp = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return otp;
        }


        private string GenerateRandomPassword()
        {
            const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";
            const string specialChars = "@#$%&-_"; // Add more special characters as needed

            var random = new Random();
            var password = new char[8]; // You can adjust the password length as needed

            // Add at least one character from each character set
            password[0] = uppercaseChars[random.Next(uppercaseChars.Length)];
            password[1] = lowercaseChars[random.Next(lowercaseChars.Length)];
            password[2] = digitChars[random.Next(digitChars.Length)];
            password[3] = specialChars[random.Next(specialChars.Length)];

            // Fill the remaining characters with a mix of all character sets
            for (int i = 4; i < password.Length; i++)
            {
                string charSet = uppercaseChars + lowercaseChars + digitChars + specialChars;
                password[i] = charSet[random.Next(charSet.Length)];
            }

            // Shuffle the characters in the password
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                char temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }

            return new string(password);
        }



    }
}
