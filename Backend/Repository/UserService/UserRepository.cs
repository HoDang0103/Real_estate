using AutoMapper;
using Backend.Models;
using Backend.Repository.UserService.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.UserService
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task LockUserAsync(string id, LockUserDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) 
            {
                throw new InvalidOperationException($"User not found");
            }
            
            var lockUsers = _mapper.Map<ApplicationUser>(model);
            _context.Users.Update(lockUsers);
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(string id, ChangePasswordDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new InvalidOperationException($"User not found");
            }
            var users = _mapper.Map<ApplicationUser>(model);
            _context.Users.Update(users);
            await _context.SaveChangesAsync();
        }
    }
}
