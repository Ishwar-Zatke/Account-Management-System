using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using Account_Management.Repository.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Channels;

namespace Account_Management.Repository.SqlCode
{
    public class CustomerRepo : ICustomerRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public CustomerRepo(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            
            using(IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    //var cust = await dbContext.Customers.FindAsync(customer.Id);
                    //var contactNumber = dbContext.Customers.Where(x => x.ContactNumber == customer.ContactNumber);
                    //if (cust != null)
                    //{
                    //    throw new CustomerExceptionHandler("Customer with same id exist", 400);
                    //}
                    //if (contactNumber != null)
                    //{
                    //    throw new CustomerExceptionHandler("Customer with same contact number exist", 400);
                    //}
                    var accountIdCheck = await dbContext.Accounts.FirstOrDefaultAsync(x=>x.AccountNumber == customer.AccountNumber);
                    
                    if (accountIdCheck == null && customer.AccountNumber!=null) {
                        throw new CustomerExceptionHandler("Account doesn't exist, Create Account first", 400);
                    }
                    await dbContext.Customers.AddAsync(customer);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    
                    return customer;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
                //var customerAccountDTO = new CustomersAccountDTO()
                //{
                //    Id = Guid.NewGuid(),
                //    CustomerId = customer.Id,
                //    AccountId = customer.AccountId,
                //};
                //var customerAccount = mapper.Map<CustomersAccount>(customerAccountDTO);
                //await dbContext.CustomersAccounts.AddAsync(customerAccount);
                //await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<Customer>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var customer = dbContext.Customers.Include("Account").AsQueryable();

                if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
                {
                    if (filterOn.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = customer.Where(x => x.Id.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("CustomerName", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = customer.Where(x => x.CustomerName.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("ContactNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = customer.Where(x => x.ContactNumber.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("Address", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = customer.Where(x => x.Address.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("AccountNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = customer.Where(x => x.AccountNumber.Equals(filterQuery));
                    }
                }

                // Sorting
                if (string.IsNullOrWhiteSpace(sortBy) == false)
                {
                    if (sortBy.Equals("CustomerName", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = isAscending ? customer.OrderBy(x => x.CustomerName) : customer.OrderByDescending(x => x.CustomerName);
                    }
                    else if (sortBy.Equals("ContactNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = isAscending ? customer.OrderBy(x => x.ContactNumber) : customer.OrderByDescending(x => x.ContactNumber);
                    }
                    else if (sortBy.Equals("Address", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = isAscending ? customer.OrderBy(x => x.Address) : customer.OrderByDescending(x => x.Address);
                    }
                    else if (sortBy.Equals("AccountNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = isAscending ? customer.OrderBy(x => x.AccountNumber) : customer.OrderByDescending(x => x.AccountNumber);
                    }
                    else if (sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        customer = isAscending ? customer.OrderBy(x => x.Id) : customer.OrderByDescending(x => x.Id);
                    }
                }

                // Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                return await customer.Skip(skipResults).Take(pageSize).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async Task<Customer?> UpdateAsync(Guid id, Customer customer)
        {
            using(IDbContextTransaction transaction1 = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingCustomer = await dbContext.Customers.FirstOrDefaultAsync(x=>x.Id == id);
                    customer.Id = id;
                    //if(existingCustomer != null) { }
                    //var dtoProperties = typeof(Customer).GetProperties();

                    //foreach (var item in dtoProperties)
                    //{
                    //    Console.WriteLine("dtoProperties : " + item);
                    //}

                    //foreach (var propertyInfo in dtoProperties)
                    //{
                    //    var dtoValue = propertyInfo.GetValue(customer);

                    //    if (dtoValue != null)
                    //    {
                    //        var customerProperty = typeof(Customer).GetProperty(propertyInfo.Name);

                    //        if (customerProperty != null)
                    //        {
                    //            customerProperty.SetValue(existingCustomer, dtoValue);
                    //            Console.WriteLine("Customer Property  : " + customerProperty);
                    //        }
                    //    }
                    //}
                    if (existingCustomer == null)
                    {
                        return null;
                    }
                    if (!string.IsNullOrWhiteSpace(customer.CustomerName))
                    {
                        existingCustomer.CustomerName = customer.CustomerName;
                    }
                    if (!string.IsNullOrWhiteSpace(customer.ContactNumber))
                    {
                        existingCustomer.ContactNumber = customer.ContactNumber;
                    }
                    if (!string.IsNullOrWhiteSpace(customer.Address))
                    {
                        existingCustomer.Address = customer.Address;
                    }
                    if (!string.IsNullOrWhiteSpace(customer.AccountNumber))
                    {
                        var account = await dbContext.Accounts.SingleOrDefaultAsync(x => x.AccountNumber == existingCustomer.AccountNumber);
                        if (account != null)
                        {
                            existingCustomer.AccountNumber = customer.AccountNumber;
                            if(account.CustomerName == null)
                            {
                                account.CustomerName = customer.CustomerName;
                            }else if(account.Address == null)
                            {
                                account.Address = customer.Address;
                            }
                        }
                        else
                        {
                            throw new CustomerExceptionHandler("Account with AccountId doesn't existed", 400);
                        }

                    }

                    await dbContext.SaveChangesAsync();
                    await transaction1.CommitAsync();
                    return existingCustomer;
                }
                catch (Exception ex)
                {
                    await transaction1.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
            
        }

        public async Task<Customer?> DeleteAsync(Guid id)
        {
            using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
                    //if (customer == null)
                    //{
                    //    throw new CustomerExceptionHandler("Customer doesn't existed", 400);
                    //}
                    dbContext.Customers.Remove(customer);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return customer;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
            
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            try
            {
                var customer = await dbContext.Customers.FindAsync(id);
                if (customer == null)
                {
                    throw new CustomerExceptionHandler("Customer doesn't existed", 400);
                }
                return customer;
            }
            catch( Exception ex )
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Account>> GetAllAccountsByCustomerIdAsync(Guid id)
        {
            try
            {
                var accountIds = await dbContext.CustomersAccounts.Where(x => x.CustomerId == id).Select(ca => ca.AccountId).ToListAsync();
                var accounts = await dbContext.Accounts.Where(x => accountIds.Contains(x.Id)).ToListAsync();
                return accounts;
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
