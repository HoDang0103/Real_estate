using Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Repository.Transaction.Dtos
{
    public class TopUpDto
    {
        public int Id { get; set; }
        public string? UserID { get; set; }
        public double AmountTransfer { get; set; }
        public DateTime TransactionDate { get; set; }
        public UserForTopUp? User { get; set; }
    }

    public class UserForTopUp
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

    }
}
