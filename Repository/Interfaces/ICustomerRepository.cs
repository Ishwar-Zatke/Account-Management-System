using Account_Management.Models.Domain;

namespace Account_Management.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer);
        Task<List<Customer>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 5);

        Task<Customer?> UpdateAsync(Guid id, Customer customer);
        Task<Customer?> DeleteAsync(Guid id);
        Task<Customer?> GetByIdAsync(Guid id);

        Task<List<Account>> GetAllAccountsByCustomerIdAsync(Guid id);
    }
}
