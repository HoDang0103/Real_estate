namespace Backend.Repository.UserService.Dtos
{
    public class GetDetailUserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TypeAccount { get; set; }
        public decimal Surplus { get; set; }
        public decimal Promotion { get; set; }
        public string? Image { get; set; }
    }
}
