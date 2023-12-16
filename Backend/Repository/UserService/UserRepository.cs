using AutoMapper;
using Backend.Models;
using Backend.Repository.UserService.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<List<GetAllUserDto>> GetAllUserAsync(int page, int pageSize)
        {
            var users = await _context.Users
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            return _mapper.Map<List<GetAllUserDto>>(users);
        }

        public async Task<GetDetailUserDto> GetUserAsync(string id)
        {
            var users = await _context.Users.FindAsync(id);
            return _mapper.Map<GetDetailUserDto>(users);
        }

        public async Task<List<GetAllUserDto>> SearchUser([FromBody] SearchUserDto searchUser)
        {
            var query = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchUser.Email))
            {
                query = query.Where(u => u.Email.Contains(searchUser.Email));
            }

            if (!string.IsNullOrEmpty(searchUser.Phone))
            {
                query = query.Where(u => u.PhoneNumber.Contains(searchUser.Phone));
            }

            if (!string.IsNullOrEmpty(searchUser.TypeAccount))
            {
                query = query.Where(u => u.TypeAccount == searchUser.TypeAccount);
            }

            var users = await query.ToListAsync();
            return _mapper.Map<List<GetAllUserDto>>(users);
        }
    }
}
