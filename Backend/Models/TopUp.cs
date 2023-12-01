using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class TopUp
    {
        public int Id { get; set; }
        public string? UserID { get; set; }
        public decimal AmountTransfer { get; set; }
        public string? Status { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser? User { get; set; }
    }
}
