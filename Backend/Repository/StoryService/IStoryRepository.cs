using Backend.Repository.StoryService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Repository.StoryService
{
    public interface IStoryRepository
    {
        public Task<int> CreateStoryAsync(string userID, CreateStoryDto model);
        public Task<StoryDto> GetStoryById(int id);
        public Task<List<StoryDto>> GetAllSaleStorysAsync(int page, int pageSize);
        public Task<List<StoryDto>> GetAllRentStorysAsync(int page, int pageSize);
        public Task<List<StoryDto>> GetAllSaleStorysByCatalogAsync(string catalogName, int page, int pageSize);
        public Task<List<StoryDto>> GetAllRentStorysByCatalogAsync(string catalogName, int page, int pageSize);
        public Task<List<StoryDto>> GetAllStorysByUserIdAsync(string userId);
        public Task<List<StoryDto>> SearchSaleStoriesAsync(SearchStoryDto searchDto, int page, int pageSize);
        public Task<List<StoryDto>> SearchRentStoriesAsync(SearchStoryDto searchDto, int page, int pageSize);
        public Task<string> RepostStoryAsync(string userId, int storyId, RepostStoryDto repostDto);
        public Task<string> DeleteStoryAsync(int storyId);
        public Task<List<StoryDto>> GetExpiredStoriesCurrentUserAsync(string userId);
    }
}
