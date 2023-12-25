using Backend.Models;
using Backend.Repository.StoryService.Dtos;

namespace Backend.Repository.StoryService
{
    public interface IStoryRepository
    {
        public Task<int> CreateStoryAsync(string userID, CreateStoryDto model);
        public Task<Story> GetStoryById(int id);
    }
}
