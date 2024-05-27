using Account_Management.Models.Domain;

namespace Account_Management.Repository.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Transaction?> GetById(Guid id);
        Task<List<Transaction>?> GetAllByAccountIdAsync(Guid AccountId);
        Task<Transaction> CreateAsync(Transaction transaction);
    }
}
