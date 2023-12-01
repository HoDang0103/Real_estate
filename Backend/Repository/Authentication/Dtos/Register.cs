using System.ComponentModel.DataAnnotations;

namespace Backend.Repository.Authentication.Dtos
{
    public class Register
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
