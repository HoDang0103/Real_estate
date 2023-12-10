using Backend.Models;
using Microsoft.AspNetCore.Identity;
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
        public static double ConvertVndToDollar(double vnd)
        {
            var total = Math.Round(vnd / ExchangeRate, 2);

            return total;
        }

        public TopUpRepository(ApplicationDbContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
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
                    ReturnUrl = "https://localhost:7015/swagger/index.html",
                    CancelUrl = "https://localhost:7015/swagger/index.html"
                }
            };
        }
    }
}
