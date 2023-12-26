using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.StoryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository _storyRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StoryController(IStoryRepository storyRepository, UserManager<ApplicationUser> userManager)
        {
            _storyRepository = storyRepository;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateStory([FromForm] CreateStoryDto model)
        {
            try
            {
                // Lấy userID từ Claims hoặc nơi bạn lưu trữ thông tin đăng nhập
                string userID = GetUserIdFromToken();
                if (userID == null)
                {
                    return BadRequest("User Not Found");
                }

                var storyId = await _storyRepository.CreateStoryAsync(userID, model);

                return CreatedAtAction(nameof(GetStory), new { id = storyId }, null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        private string GetUserIdFromToken()
        {
            return _userManager.GetUserId(HttpContext.User);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStory(int id)
        {
            try
            {
                // Lấy thông tin câu chuyện từ repository hoặc dịch vụ của bạn
                var story = await _storyRepository.GetStoryById(id);

                if (story == null)
                {
                    return NotFound(); // Trả về mã lỗi 404 nếu câu chuyện không được tìm thấy
                }

                // Trả về thông tin câu chuyện trong response
                return Ok(story);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllSaleStories")]
        public async Task<IActionResult> GetAllSaleStories(int page, int pageSize)
        {
            try
            {
                var stories = await _storyRepository.GetAllSaleStorysAsync(page, pageSize);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllRentStories")]
        public async Task<IActionResult> GetAllRentStories(int page, int pageSize)
        {
            try
            {
                var stories = await _storyRepository.GetAllRentStorysAsync(page, pageSize);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
