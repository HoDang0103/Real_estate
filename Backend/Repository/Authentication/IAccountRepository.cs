using Backend.Repository.Authentication.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend.Repository.Authentication
{
    public interface IAccountRepository
    {
        public Task<IdentityResult> RegisterUser(Register model);
        public Task<IdentityResult> RegisterAdmin(Register model);
        public Task<string> Login(Login model);
    }
}
