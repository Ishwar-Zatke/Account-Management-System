using Account_Management.Models.Domain;

namespace Account_Management.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account> CreateAsync(Account account);
        Task<List<Account>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);

        Task<Account?> UpdateAsync(Guid id, Account account);
        Task<Account?> DeleteAsync(Guid id);
        Task<Account?> GetByIdAsync(Guid id);

        Task<List<Customer>> GetAllCustomerByAccountIdAsync(Guid id);
        Task<List<Transaction>> GetAllTransactionByAccountIdAsync(Guid id);
    }
}
