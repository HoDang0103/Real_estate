namespace Backend.Repository.UserService.Dtos
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
