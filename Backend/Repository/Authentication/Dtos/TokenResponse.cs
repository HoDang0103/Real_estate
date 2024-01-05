namespace Backend.Repository.Authentication.Dtos
{
    public class TokenResponse
    {
        public string? Token { get; set; }
        public DateTime Expiry { get; set; }
        public string? UserId { get; set; }
        public string? Status { get; set; }
    }
}
