using Microsoft.AspNetCore.Identity;

namespace Backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public decimal Surplus { get; set; }
        public decimal Promotion { get; set; }
        public string? TypeAccount { get; set; }
        public string? Code { get; set; }
        public string? Image { get; set; }
        public string? OTP { get; set; }
        public virtual ICollection<Story> Stories { get; set; } = new List<Story>();
        public virtual ICollection<TopUp> TopUps { get; set; } = new List<TopUp>();
        public virtual ICollection<Notify> Notifies { get; set; } = new List<Notify>();
    }
}
