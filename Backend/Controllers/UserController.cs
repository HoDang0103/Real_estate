using Backend.Mapper;
using Backend.Models;
using Backend.Repository.UserService;
using Backend.Repository.UserService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        [Authorize(Roles = AppRole.Admin)]
        [HttpPut("LockOrUnLockUser")]
        public async Task<IActionResult> LockUser(string id, LockUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (model.LockoutEnabled.HasValue)
            {
                user.LockoutEnabled = model.LockoutEnabled.Value;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("User updated successfully.");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordDto model)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return BadRequest("New password and confirm password do not match.");
            }

            var success = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (success.Succeeded)
            {
                return Ok("User updated successfully.");
            }
            else
            {
                return BadRequest(success);
            }
        }
    }
}
