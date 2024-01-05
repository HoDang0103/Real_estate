using AutoMapper;
using Backend.Models;
using Backend.Repository.UserService.Dtos;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserRepository(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        public async Task<UpdateUserDto> UpdateUserAsync(string userId, UpdateUserDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return null;
                }

                // Cập nhật thông tin người dùng
                user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;

                // Cập nhật hình ảnh nếu được cung cấp
                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var extension = Path.GetExtension(model.Image.FileName);
                    var fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}{extension}";
                    var filePath = Path.Combine(uploadsFolderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    // Cập nhật đường dẫn hình ảnh vào Image của ApplicationUser
                    user.Image = $"images/{fileName}";
                }

                // Lưu lại thông tin người dùng sau khi cập nhật
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return new UpdateUserDto
                    {
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        Image = model.Image
                    };
                }
                return null;
            }
            catch
            {
                // Xử lý ngoại lệ hoặc logging
                return null;
            }
        }

        public async Task<GetDetailUserDto> GetCurrentUserAsync(string id)
        {
            var users = await _userManager.FindByIdAsync(id);
            return _mapper.Map<GetDetailUserDto>(users);
        }
    }
}
