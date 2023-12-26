using AutoMapper;
using AutoMapper.Configuration.Conventions;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

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
                StartDate = model.StartDate,
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
                // Map các thuộc tính khác
                Images = _mapper.Map<List<ImageDto>>(story.Images),
                User = _mapper.Map<ApplicationUserDto>(story.User)
            };

            return storyDto;
        }

        public async Task<List<StoryDto>> GetAllSaleStorysAsync(int page, int pageSize)
        {
            var stories = await _context.Stories
                .Where(s => s.Needs == true)
                .Include(s => s.Catalog)
                .Include(s => s.Images)
                .Include(s => s.User)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<Story>, List<StoryDto>>(stories);

            return storyDtos;
        }

        public async Task<List<StoryDto>> GetAllRentStorysAsync(int page, int pageSize)
        {
            var skipAmount = pageSize * (page - 1);

            var stories = await _context.Stories
                .Where(s => s.Needs == false) // Lọc các câu chuyện có Needs là true
                .OrderByDescending(s => s.CreatedAt) // Sắp xếp theo CreatedAt giảm dần
                .Skip(skipAmount)
                .Take(pageSize)
                .ToListAsync();

            var storyDtos = _mapper.Map<List<StoryDto>>(stories);

            return storyDtos;
        }
    }
}
