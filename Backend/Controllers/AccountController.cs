using Backend.Repository.Authentication;
using Backend.Repository.Authentication.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository accountRepo;

        public AccountController(IAccountRepository repo)
        {
            accountRepo = repo;
        }

        [HttpPost("Register-User")]
        public async Task<IActionResult> Register_User(Register signUpModel)
        {
            var result = await accountRepo.RegisterUser(signUpModel);
            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }

            return BadRequest("Username or Email has been registered.");
        }

        [HttpPost("Register-Admin")]
        public async Task<IActionResult> Register_Admin(Register signUpModel)
        {
            var result = await accountRepo.RegisterAdmin(signUpModel);
            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }

            return BadRequest("Username or Email has been registered.");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login signInModel)
        {
            var result = await accountRepo.Login(signInModel);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Username or Password incorrect.");
            }
            if (result == "Locked")
            {
                return BadRequest("Account is locked.");
            }
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await accountRepo.LogOutAsync();

                return Ok("Logout successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
