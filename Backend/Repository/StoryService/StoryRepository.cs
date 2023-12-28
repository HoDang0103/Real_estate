using AutoMapper;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository.StoryService
{
    public class StoryRepository : IStoryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;


        public StoryRepository(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<int> CreateStoryAsync(string userID, CreateStoryDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userID);
            if (user == null) 
            {
                throw new InvalidOperationException($"User not found");
            }

            var package = await _context.Packages.FindAsync(model.PackageID);
            if (package == null)
            {
                throw new InvalidOperationException($"Package not found");
            }
            var catagory = await _context.Catalogs.FindAsync(model.CatalogID);
            if (catagory == null) 
            {
                throw new InvalidOperationException($"Catagory not found");
            }
            var story = new Story
            {
                CatalogID = model.CatalogID,
                PackageID = model.PackageID,
                Needs = model.Needs,
                Title = model.Title,
                Description = model.Description,
                Save = false,
                Floor = model.Floor,
                District = model.District,
                Ward = model.Ward,
                Street = model.Street,
                Project = model.Project,
                Address = model.Project + ", " + model.Street + ", " + model.Ward + ", " + model.District,
                Location = model.Location,
                Area = model.Area,
                Price = model.Price,
                Unit = model.Unit,
                Document = model.Document,
                Interior = model.Interior,
                Bedrooms = model.Bedrooms,
                WC = model.WC,
                State = model.State,
                StartDate = DateTime.Now,
                CreatedAt = DateTime.Now, // Thay đổi để sử dụng thời gian hiện tại của server
                UpdatedAt = DateTime.Now, // Thay đổi để sử dụng thời gian hiện tại của server
                EndDate = model.StartDate.AddDays(package.NumberDay),
                IsActive = true,
                UserID = userID
            };

            foreach (var imageFile in model.ImageFiles)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imagePath = await SaveImageAsync(imageFile);

                    var image = new Image
                    {
                        ImageLink = imagePath,
                    };

                    story.Images.Add(image);
                }
            }

            // Trừ giá trị từ Package vào Promotion và Surplus
            if (user.Promotion >= package.PricePerDay * package.NumberDay)
            {
                user.Promotion -= package.PricePerDay * package.NumberDay;
            }
            else if (user.Surplus >= package.PricePerDay * package.NumberDay)
            {
                user.Surplus -= package.PricePerDay * package.NumberDay;
            }
            else
            {
                // Trả về thông báo lỗi nếu không đủ tiền
                throw new InvalidOperationException("Tài khoản quý khách không đủ số dư. Vui lòng nạp thêm tiền vào tài khoản");
            }

            _context.Stories.Add(story);
            await _context.SaveChangesAsync();

            return story.Id;
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException("file", "No file uploaded.");
            }

            // Lấy đường dẫn đầy đủ của thư mục wwwroot/images
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Tạo thư mục nếu nó chưa tồn tại
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Lấy tên tệp gốc của tệp tin
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);

            // Tạo tên tệp mới dựa trên ngày giờ
            var uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";

            // Đường dẫn đầy đủ của tệp tin
            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Lưu tệp tin
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn đến tệp tin đã lưu
            return $"/images/{uniqueFileName}";
        }

        public async Task<StoryDto> GetStoryById(int id)
        {
            var story = await _context.Stories
                 .Include(s => s.Images)
                 .Include(s => s.User)
                 .FirstOrDefaultAsync(s => s.Id == id);

            if (story == null)
            {
                return null;
            }

            var storyDto = new StoryDto
            {
                Id = story.Id,
                Needs = story.Needs,
                Title = story.Title,
                Description = story.Description,
                Floor = story.Floor,
                Address = story.Address,
                Area = story.Area,
                Price = story.Price,
                Unit = story.Unit,
                Document = story.Document,
                Interior = story.Interior,
                Bedrooms = story.Bedrooms,
                WC = story.WC,
                State = story.State,
                StartDate = story.StartDate,
                CreatedAt = story.CreatedAt,
                EndDate = story.EndDate,
                Images = _mapper.Map<List<ImageDto>>(story.Images),
                User = _mapper.Map<ApplicationUserDto>(story.User)
            };

            return storyDto;
        }

        public async Task<List<StoryDto>> GetAllSaleStorysAsync(int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var stories = await _context.Stories
                .Where(s => s.Needs == true && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                       s.Package.Name == "Gold" ? 2 :
                       s.Package.Name == "Silver" ? 3 :
                       s.Package.Name == "Basic" ? 4 :
                       s.Package.Name == "Free" ? 5 : 6)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> GetAllRentStorysAsync(int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var stories = await _context.Stories
                .Where(s => s.Needs == false && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                       s.Package.Name == "Gold" ? 2 :
                       s.Package.Name == "Silver" ? 3 :
                       s.Package.Name == "Basic" ? 4 :
                       s.Package.Name == "Free" ? 5 : 6)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> GetAllSaleStorysByCatalogAsync(string catalogName, int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var stories = await _context.Stories
                .Where(s => s.Catalog.CatalogName == catalogName && s.Needs == true && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                              s.Package.Name == "Gold" ? 2 :
                              s.Package.Name == "Silver" ? 3 :
                              s.Package.Name == "Basic" ? 4 :
                              s.Package.Name == "Free" ? 5 : 6)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> GetAllRentStorysByCatalogAsync(string catalogName, int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var stories = await _context.Stories
                .Where(s => s.Catalog.CatalogName == catalogName && s.Needs == false && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                              s.Package.Name == "Gold" ? 2 :
                              s.Package.Name == "Silver" ? 3 :
                              s.Package.Name == "Basic" ? 4 :
                              s.Package.Name == "Free" ? 5 : 6)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> GetAllStorysByUserIdAsync(string userId)
        {
            // Lấy tất cả câu chuyện của người dùng với ID tương ứng
            var stories = await _context.Stories
                .Where(s => s.UserID == userId)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                              s.Package.Name == "Gold" ? 2 :
                              s.Package.Name == "Silver" ? 3 :
                              s.Package.Name == "Basic" ? 4 :
                              s.Package.Name == "Free" ? 5 : 6)
                .ToListAsync();

            // Map các câu chuyện sang Dto
            var storyDtos = _mapper.Map<List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> SearchSaleStoriesAsync(SearchStoryDto searchDto, int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var query = _context.Stories
                .Where(s => s.Needs == true && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                              s.Package.Name == "Gold" ? 2 :
                              s.Package.Name == "Silver" ? 3 :
                              s.Package.Name == "Basic" ? 4 :
                              s.Package.Name == "Free" ? 5 : 6)
                .AsQueryable();

            if (searchDto.District != null)
            {
                query = query.Where(s => s.District == searchDto.District);
            }

            if (searchDto.MinPrice != null)
            {
                query = query.Where(s => s.Price >= searchDto.MinPrice);
            }

            if (searchDto.MaxPrice != null)
            {
                query = query.Where(s => s.Price <= searchDto.MaxPrice);
            }

            if (searchDto.MinArea != null)
            {
                query = query.Where(s => s.Area >= searchDto.MinArea);
            }

            if (searchDto.MaxArea != null)
            {
                query = query.Where(s => s.Area <= searchDto.MaxArea);
            }

            if (searchDto.Title != null)
            {
                query = query.Where(s => s.Title.Contains(searchDto.Title));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedQuery = query.Skip((page - 1) * pageSize).Take(pageSize);

            var filteredStories = await pagedQuery.ToListAsync();
            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(filteredStories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> SearchRentStoriesAsync(SearchStoryDto searchDto, int page, int pageSize)
        {
            var currentDate = DateTime.Now;
            var query = _context.Stories
                .Where(s => s.Needs == false && s.EndDate > currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .OrderBy(s => s.Package.Name == "Diamond" ? 1 :
                              s.Package.Name == "Gold" ? 2 :
                              s.Package.Name == "Silver" ? 3 :
                              s.Package.Name == "Basic" ? 4 :
                              s.Package.Name == "Free" ? 5 : 6)
                .AsQueryable();

            if (searchDto.District != null)
            {
                query = query.Where(s => s.District == searchDto.District);
            }

            if (searchDto.MinPrice != null)
            {
                query = query.Where(s => s.Price >= searchDto.MinPrice);
            }

            if (searchDto.MaxPrice != null)
            {
                query = query.Where(s => s.Price <= searchDto.MaxPrice);
            }

            if (searchDto.MinArea != null)
            {
                query = query.Where(s => s.Area >= searchDto.MinArea);
            }

            if (searchDto.MaxArea != null)
            {
                query = query.Where(s => s.Area <= searchDto.MaxArea);
            }

            if (searchDto.Title != null)
            {
                query = query.Where(s => s.Title.Contains(searchDto.Title));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var pagedQuery = query.Skip((page - 1) * pageSize).Take(pageSize);

            var filteredStories = await pagedQuery.ToListAsync();
            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(filteredStories);

            return storyDtos;
        }

        public async Task<string> RepostStoryAsync(string userId, int storyId, RepostStoryDto repostDto)
        {
            try
            {
                var story = await _context.Stories
                    .Include(s => s.Package)
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == storyId);

                if (story == null)
                {
                    return "NotFound";
                }

                // Chỉ cho phép chỉnh sửa PackageID và StartDate
                story.PackageID = repostDto.PackageID;
                story.StartDate = repostDto.StartDate;

                if (!IsValidRepost(story))
                {
                    return "Fail";
                }

                // Kiểm tra xem người đăng lại có đúng là chủ sở hữu của câu chuyện hay không
                if (story.UserID != userId)
                {
                    return "Unauthorized";
                }

                // Tính toán lại EndDate dựa trên StartDate và NumberDay
                story.EndDate = story.StartDate.AddDays(story.Package.NumberDay);

                // Tính toán số tiền bị trừ
                decimal deductionAmount = story.Package.PricePerDay * story.Package.NumberDay;

                // Kiểm tra và trừ tiền từ tài khoản Promotion hoặc Surplus
                if (TryDeductMoney(story.User, deductionAmount))
                {
                    await _context.SaveChangesAsync();
                    return "Success";
                }

                return "NoMoney";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsValidRepost(Story story)
        {
            return story.Package != null && story.User != null && story.Package.NumberDay > 0;
        }

        private bool TryDeductMoney(ApplicationUser user, decimal amount)
        {
            if (user.Promotion >= amount)
            {
                user.Promotion -= amount;
                return true;
            }

            if (user.Surplus >= amount)
            {
                user.Surplus -= amount;
                return true;
            }

            return false;
        }

        public async Task<string> DeleteStoryAsync(int storyId)
        {
            try
            {
                var story = await _context.Stories
                    .Include(s => s.Images)
                    .FirstOrDefaultAsync(s => s.Id == storyId);

                if (story == null)
                {
                    return "NotFound";
                }

                // Xóa hình ảnh liên quan đến câu chuyện
                _context.Images.RemoveRange(story.Images);

                // Xóa câu chuyện
                _context.Stories.Remove(story);

                await _context.SaveChangesAsync();

                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<StoryDto>> GetExpiredStoriesCurrentUserAsync(string userId)
        {
            var currentDate = DateTime.Now;
            var stories = await _context.Stories
                .Where(s => s.UserID == userId && s.EndDate < currentDate)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Include(s => s.Package)
                .ToListAsync();

            // Map các câu chuyện sang Dto
            var storyDtos = _mapper.Map<List<StoryDto>>(stories);

            return storyDtos;
        }
    }
}
