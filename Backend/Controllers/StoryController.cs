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
using Microsoft.AspNetCore.Hosting;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoryController : ControllerBase
    {
        private readonly IStoryRepository _storyRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StoryController(IStoryRepository storyRepository, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _storyRepository = storyRepository;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpGet("GetAllSaleStoriesByCatagoryName")]
        public async Task<IActionResult> GetAllStoriesByCatagory(string catalogyName, int page, int pageSize)
        {
            try
            {
                var stories = await _storyRepository.GetAllSaleStorysByCatalogAsync(catalogyName, page, pageSize);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllRentStoriesByCatagoryName")]
        public async Task<IActionResult> GetAllRentStoriesByCatagory(string catalogyName, int page, int pageSize)
        {
            try
            {
                var stories = await _storyRepository.GetAllRentStorysByCatalogAsync(catalogyName, page, pageSize);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllStoryCurrentUser")]
        public async Task<IActionResult> GetAllStoryCurrentUser()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User Not Found");
                }

                // Gọi phương thức mới trong repository để lấy tất cả các câu chuyện của người dùng hiện tại
                var stories = await _storyRepository.GetAllStorysByUserIdAsync(userId);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SearchSaleStory")]
        public async Task<IActionResult> SearchStories([FromQuery] SearchStoryDto searchDto, int page, int pageSize)
        {
            try
            {
                var filteredStories = await _storyRepository.SearchSaleStoriesAsync(searchDto, page, pageSize);

                return Ok(filteredStories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("SearchRentStory")]
        public async Task<IActionResult> SearchRentStories([FromQuery] SearchStoryDto searchDto, int page, int pageSize)
        {
            try
            {
                var filteredStories = await _storyRepository.SearchRentStoriesAsync(searchDto, page, pageSize);

                return Ok(filteredStories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("Repost/{id}")]
        public async Task<IActionResult> RepostStory(int id, [FromBody] RepostStoryDto repostDto)
        {
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var result = await _storyRepository.RepostStoryAsync(userId, id, repostDto);

                switch (result)
                {
                    case "NotFound":
                        return NotFound(new { message = "Story not found." });
                    case "NoMoney":
                        return BadRequest(new { message = "Tài khoản của quý khách không đủ số dư. Vui lòng nạp thêm tiền vào tài khoản để tiếp tục." });
                    case "Unauthorized":
                        return Unauthorized(new { message = "Bạn không có quyền đăng lại tin này." });
                    case "Success":
                        return Ok(new { message = "Story reposted successfully." });
                    default:
                        return BadRequest(new { message = "Invalid Information." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            try
            {
                var result = await _storyRepository.DeleteStoryAsync(id);

                return result switch
                {
                    "NotFound" => NotFound("Story not found."),
                    "Success" => Ok("Story deleted successfully."),
                    _ => BadRequest("Invalid Information.")
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllExpiredStoryCurrentUser")]
        public async Task<IActionResult> GetAllExpiredStoryCurrentUser()
        {
            try
            {
                var userId = _userManager.GetUserId(HttpContext.User);

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User Not Found");
                }

                // Gọi phương thức mới trong repository để lấy tất cả các câu chuyện của người dùng hiện tại
                var stories = await _storyRepository.GetExpiredStoriesCurrentUserAsync(userId);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
