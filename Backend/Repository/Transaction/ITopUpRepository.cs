using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.Transaction.Dtos;

namespace Backend.Repository.Transaction
{
    public interface ITopUpRepository
    {
        public Task<string> CreateTopUpTransaction(TopUp model);
        public Task<List<TopUpDto>> GetAllTopUpsAsync(int page, int pageSize);
        public Task<TopUpDto> GetTopUpById(int id);
        public Task<List<TopUp>> GetAllTopUpsByUserId(string userId);
        public Dictionary<DateTime, double> GetRevenueStatisticsByYear(int year);
        public Dictionary<DateTime, double> GetRevenueStatisticsByMonth(int year, int month);

    }
}
