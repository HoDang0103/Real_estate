using AutoMapper;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.Transaction.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace Backend.Repository.Transaction
{
    public class TopUpRepository : ITopUpRepository
    {
        private readonly ApplicationDbContext _context;
        private const double ExchangeRate = 25000;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public static double ConvertVndToDollar(double vnd)
        {
            var total = Math.Round(vnd / ExchangeRate, 2);

            return total;
        }

        public TopUpRepository(ApplicationDbContext context, IConfiguration configuration, IMapper mapper) 
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<string> CreateTopUpTransaction(TopUp model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserID);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(model.UserID));
            }
            var envSandbox = new SandboxEnvironment(
                _configuration["PayPalSetting:ClientId"],
                _configuration["PayPalSetting:SecretId"]);
            
            var client = new PayPalHttpClient(envSandbox);
            //var paypalOrderId = DateTime.Now.Ticks;
            //var urlCallBack = _configuration["PatPalSetting:ReturnUrl"];
            var topUp = new TopUp()
            {
                UserID = model.UserID,
                AmountTransfer = ConvertVndToDollar(model.AmountTransfer),
                TransactionDate = DateTime.Now,
            };
            _context.TopUps.Add(topUp);
            user.Surplus += (decimal)model.AmountTransfer;
            await _context.SaveChangesAsync();

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody((decimal)model.AmountTransfer));
            try
            {
                var response = await client.Execute(request);
                var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

                foreach (var link in result.Links)
                {
                    if (link.Rel.ToLower().Trim().Equals("approve"))
                    {
                        return link.Href;
                    }
                }

                return string.Empty;
            }
            catch (HttpException ex)
            {
                // Xử lý lỗi từ PayPal
                Console.WriteLine(ex.StatusCode);
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private OrderRequest BuildRequestBody(decimal amount)
        {
            var usdAmount = ConvertVndToDollar((double)amount);
            return new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = usdAmount.ToString("0.00"),
                    }
                }
            },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = "http://localhost:4200/success",
                    CancelUrl = "https://localhost:7015/swagger/index.html"
                }
            };
        }

        public async Task<TopUpDto> GetTopUpById(int id)
        {
            var topup = await _context.TopUps
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (topup == null)
            {
                return null;
            }

            var topupDto = new TopUpDto
            {
                Id = topup.Id,
                AmountTransfer = topup.AmountTransfer * 25000,
                TransactionDate = topup.TransactionDate,
                User = _mapper.Map<UserForTopUp>(topup.User) 
            };

            return topupDto;
        }

        public async Task<List<TopUpDto>> GetAllTopUpsAsync(int page, int pageSize)
        {
            var topups = await _context.TopUps
                .Include(s => s.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var topupDtos = _mapper.Map<List<TopUp>, List<TopUpDto>>(topups);

            return topupDtos;
        }

        public async Task<List<TopUp>> GetAllTopUpsByUserId(string userId)
        {
            return await _context.TopUps
                .Include(t => t.User)
                .Where(t => t.User.Id == userId)
                .ToListAsync();
        }

        public Dictionary<DateTime, double> GetRevenueStatisticsByYear(int year)
        {
            var allMonths = Enumerable.Range(1, 12);

            var revenueStatistics = _context.TopUps
                .Where(r => r.TransactionDate.Year == year)
                .GroupBy(r => new { Year = r.TransactionDate.Year, Month = r.TransactionDate.Month })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    TotalAmount = g.Sum(r => r.AmountTransfer)
                })
                .ToDictionary(r => r.Date, r => ConvertToVND(r.TotalAmount));

            var missingMonths = allMonths.Except(revenueStatistics.Select(r => r.Key.Month));

            foreach (var month in missingMonths)
            {
                var missingDate = new DateTime(year, month, 1);
                revenueStatistics[missingDate] = 0;
            }

            return revenueStatistics;
        }

        public Dictionary<DateTime, double> GetRevenueStatisticsByMonth(int year, int month)
        {
            var daysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(year, month));

            var topUps = _context.TopUps
                .Where(r => r.TransactionDate.Year == year && r.TransactionDate.Month == month)
                .ToList();

            var revenueStatistics = daysInMonth
                .Select(day => new DateTime(year, month, day))
                .ToDictionary(date => date, date =>
                {
                    var totalAmountUSD = topUps
                        .Where(r => r.TransactionDate.Day == date.Day)
                        .Sum(r => r.AmountTransfer);

                    return ConvertToVND(totalAmountUSD);
                });

            return revenueStatistics;
        }
        private double ConvertToVND(double amountUSD)
        {
            var exchangeRate = 25000; // Tỷ giá chuyển đổi từ USD sang VND
            return amountUSD * exchangeRate;
        }
    }
}
