using Backend.Models;
using Backend.Repository.Transaction;
using Backend.Repository.Transaction.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public TopUpController(ITopUpRepository topUpRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _topUpRepository = topUpRepository;
            _userManager = userManager;
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> TopUp(TopUpRequest request)
        {
            try
            {
                // Thực hiện validation trên request model nếu cần

                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                // Gọi API PayPal để bắt đầu quá trình thanh toán
                var paypalUrl = await _topUpRepository.CreateTopUpTransaction(new TopUp
                {
                    UserID = request.UserId,
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
    }
}
