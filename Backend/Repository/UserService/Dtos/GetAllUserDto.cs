namespace Backend.Repository.UserService.Dtos
{
    public class GetAllUserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TypeAccount { get; set; }
    }
}
