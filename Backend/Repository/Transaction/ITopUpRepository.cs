using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Backend.Repository.Transaction.Dtos;

namespace Backend.Repository.Transaction
{
    public interface ITopUpRepository
    {
        Task<string> CreateTopUpTransaction(TopUp model);
        public Task<List<TopUpDto>> GetAllTopUpsAsync(int page, int pageSize);
        public Task<TopUpDto> GetTopUpById(int id);
        public Task<List<TopUp>> GetAllTopUpsByUserId(string userId);


    }
}
