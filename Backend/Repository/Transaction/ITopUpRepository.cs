using Backend.Models;
using Backend.Repository.Transaction.Dtos;

namespace Backend.Repository.Transaction
{
    public interface ITopUpRepository
    {
        Task<string> CreateTopUpTransaction(TopUp model);
    }
}
