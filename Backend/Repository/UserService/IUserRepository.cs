using Backend.Repository.UserService.Dtos;

namespace Backend.Repository.UserService
{
    public interface IUserRepository
    {
        public Task LockUserAsync(string id, LockUserDto model);
        public Task ChangePasswordAsync(string id, ChangePasswordDto model);

    }
}
