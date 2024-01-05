using AutoMapper;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.Transaction;
using Backend.Repository.Transaction.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpRepository _topUpRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TopUpController(ITopUpRepository topUpRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper)
        {
            _topUpRepository = topUpRepository;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TopUp(TopUpRequest request)
        {
            try
            {
                // Thực hiện validation trên request model nếu cần

                // Lấy thông tin người dùng hiện tại
                var userId = _userManager.GetUserId(User);
                if (userId == null)
                {
                    return NotFound("User not found");
                }

                // Gọi API PayPal để bắt đầu quá trình thanh toán
                var paypalUrl = await _topUpRepository.CreateTopUpTransaction(new TopUp
                {
                    UserID = userId,
                    AmountTransfer = request.Amount
                });

                // Xử lý phản hồi từ API PayPal
                if (!string.IsNullOrEmpty(paypalUrl))
                {
                    // Nếu thành công, trả về URL để chuyển hướng người dùng đến trang thanh toán PayPal
                    return Ok(new { PaypalUrl = paypalUrl });
                }
                else
                {
                    return BadRequest("Failed to initiate PayPal transaction");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetAllTopUps")]
        public async Task<IActionResult> GetAllTopUps(int page, int pageSize)
        {
            try
            {
                var topups = await _topUpRepository.GetAllTopUpsAsync(page, pageSize);
                topups.ForEach(topUp => topUp.AmountTransfer *= 25000);

                return Ok(topups);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTopUp(int id)
        {
            try
            {
                var story = await _topUpRepository.GetTopUpById(id);

                if (story == null)
                {
                    return NotFound(); 
                }

                return Ok(story);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("GetAllTopUpByCurrentUserId")]
        public async Task<IActionResult> GetAllTopUpByCurrentUserId()
        {
            try
            {
                // Lấy ID của người dùng đang đăng nhập
                var currentUserId = _userManager.GetUserId(User);

                // Lấy tất cả các TopUps của người dùng đang đăng nhập từ repository
                var topUps = await _topUpRepository.GetAllTopUpsByUserId(currentUserId);
                topUps.ForEach(topUp => topUp.AmountTransfer *= 25000);

                // Chuyển đổi danh sách TopUps sang DTO nếu cần
                var topUpDtos = _mapper.Map<List<MyTopUpDto>>(topUps);

                return Ok(topUpDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-revenue-statistics/{year}")]
        public ActionResult<Dictionary<string, double>> GetRevenueStatistics(int year)
        {
            var revenueStatistics = _topUpRepository.GetRevenueStatisticsByYear(year)
                .ToDictionary(
                    kvp => kvp.Key.ToString("yyyy-MM"),
                    kvp => kvp.Value
                );

            return Ok(revenueStatistics);
        }

        [HttpGet("get-revenue-statistics-month")]
        public ActionResult<Dictionary<DateTime, double>> GetRevenueStatistics([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var revenueStatistics = _topUpRepository.GetRevenueStatisticsByMonth(year, month);
                return Ok(revenueStatistics);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
