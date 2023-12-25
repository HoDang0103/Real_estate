using AutoMapper;
using Backend.Models;
using Backend.Repository.StoryService.Dtos;
using Microsoft.AspNetCore.Identity;
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
            var story = new Story
            {
                CatalogID = model.CatalogID,
                PackageID = model.PackageID,
                Needs = model.Needs,
                Title = model.Title,
                Description = model.Description,
                Save = model.Save,
                Floor = model.Floor,
                Address = model.Address,
                District = model.District,
                Ward = model.Ward,
                Street = model.Street,
                Project = model.Project,
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

        public async Task<Story> GetStoryById(int id)
        {
            return await _context.Stories.FindAsync(id);
        }
    }
}
