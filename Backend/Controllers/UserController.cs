using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _mapper = mapper;
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
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return BadRequest("User Not Found");
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
            catch (Exception ex) 
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers(int page, int pageSize)
        {
            try
            {
                return Ok(await _userRepository.GetAllUserAsync(page, pageSize));
            }
            catch
            {
                return BadRequest();
            }
        }


        [Authorize]
        [HttpGet("GetUserBy/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            var userDto = _mapper.Map<GetDetailUserDto>(user);
            return Ok(userDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUserDto searchUser)
        {
            try
            {
                var usersDto = await _userRepository.SearchUser(searchUser);
                return Ok(usersDto);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateProfileUser")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                if (userId == null)
                {
                    return BadRequest("User Not Found");
                }
                var result = await _userRepository.UpdateUserAsync(userId, model);

                if (result)
                {
                    return Ok("User updated successfully.");
                }
                else
                {
                    return NotFound($"User with ID {userId} not found.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
