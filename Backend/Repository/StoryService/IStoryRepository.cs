using Backend.Models;
using Backend.Repository.StoryService.Dtos;

namespace Backend.Repository.StoryService
{
    public interface IStoryRepository
    {
        public Task<int> CreateStoryAsync(string userID, CreateStoryDto model);
        public Task<StoryDto> GetStoryById(int id);
        public Task<List<StoryDto>> GetAllSaleStorysAsync(int page, int pageSize);
        public Task<List<StoryDto>> GetAllRentStorysAsync(int page, int pageSize);

    }
}
