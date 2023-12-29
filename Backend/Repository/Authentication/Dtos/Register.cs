using System.ComponentModel.DataAnnotations;

namespace Backend.Repository.Authentication.Dtos
{
    public class Register
    {
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }


        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be 10 digits")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "FullName is required")]
        public string? FullName { get; set; }
    }
}
