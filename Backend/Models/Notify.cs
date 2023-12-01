using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Notify
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
    }
}
