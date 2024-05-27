using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using Account_Management.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Account_Management.Repository.SqlCode
{
    public class AccountRepo : IAccountRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ICustomerRepository customerRepo;

        public AccountRepo(ApplicationDbContext dbContext, IMapper mapper, ICustomerRepository customerRepo)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.customerRepo = customerRepo;
        }

        public async Task<Account> CreateAsync(Account account)
        {
            //var id = account.CustomerId;
            //var customer = await dbContext.Customers.FindAsync(id);
            //if(customer == null)
            //{
            //    return null;
            //}
            
            using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                   
                    var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == account.ContactNumber);
                    if (customer == null)
                    {
                        //if customer is not present create new
                        var Newcustomer = new Customer()
                        {
                            CustomerName = account.CustomerName,
                            ContactNumber = account.ContactNumber,
                            Address = account.Address,
                            AccountNumber = account.AccountNumber
                        };
                        await dbContext.Accounts.AddAsync(account);
                        await dbContext.SaveChangesAsync();
                        //var NewCustomer = await customerRepo.CreateAsync(Newcustomer);
                        
                        await dbContext.Customers.AddAsync(Newcustomer);
                        await dbContext.SaveChangesAsync();
                        //Unique key as Contact Number used
                        //account.ContactNumber = NewCustomer.ContactNumber;
                    }
                    else
                    {
                        //Check if Customer name is changed - throw error
                        if (account.CustomerName != customer.CustomerName)
                        {
                            throw new CustomerExceptionHandler("Customer Name Not matched with existing Customer", 400);
                        }
                        //Take Changed Address then
                        if (account.Address != null)
                        {
                            customer.Address = account.Address;
                        }
                        else
                        {
                            account.Address = customer.Address;
                        }
                        await dbContext.Accounts.AddAsync(account);
                        await dbContext.SaveChangesAsync();
                        //await customerRepo.UpdateAsync(customer.Id, customer);
                        var existingCustomer = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == account.ContactNumber); ;
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
                            existingCustomer.AccountNumber = customer.AccountNumber;
                        }

                        await dbContext.SaveChangesAsync();

                    }
                    var CustomerIdCheck = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == account.ContactNumber);
                    var customerAccountDTO = new CustomersAccountDTO()
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = CustomerIdCheck.Id,
                        AccountId = account.Id
                    };
                    var customerAccount = mapper.Map<CustomersAccount>(customerAccountDTO);
                    await dbContext.CustomersAccounts.AddAsync(customerAccount);
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return account;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }

            
            

        }

        public async Task<List<Account>> GetAllAsync(string? filterOn, string? filterQuery, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            try
            {
                var account = dbContext.Accounts.Include("AccountType").Include("Customer").AsQueryable();
                if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
                {
                    if (filterOn.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        account = account.Where(x => x.Id.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("AccountNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        account = account.Where(x => x.AccountNumber.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("Balance", StringComparison.OrdinalIgnoreCase))
                    {
                        account = account.Where(x => x.Balance.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("ContactNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        account = account.Where(x => x.ContactNumber.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("AccountTypeId", StringComparison.OrdinalIgnoreCase))
                    {
                        account = account.Where(x => x.AccountTypeId.Equals(filterQuery));
                    }
                }

                //Sorting
                if (string.IsNullOrWhiteSpace(sortBy) == false)
                {
                    if (sortBy.Equals("AccountNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        account = isAscending ? account.OrderBy(x => x.AccountNumber) : account.OrderByDescending(x => x.AccountNumber);
                    }
                    else if (sortBy.Equals("Balance", StringComparison.OrdinalIgnoreCase))
                    {
                        account = isAscending ? account.OrderBy(x => x.Balance) : account.OrderByDescending(x => x.Balance);
                    }
                    else if (sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        account = isAscending ? account.OrderBy(x => x.Id) : account.OrderByDescending(x => x.Id);
                    }
                    else if (sortBy.Equals("AccountTypeId", StringComparison.OrdinalIgnoreCase))
                    {
                        account = isAscending ? account.OrderBy(x => x.AccountTypeId) : account.OrderByDescending(x => x.AccountTypeId);
                    }
                    else if (sortBy.Equals("ContactNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        account = isAscending ? account.OrderBy(x => x.ContactNumber) : account.OrderByDescending(x => x.ContactNumber);
                    }
                }


                //Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                return await account.Skip(skipResults).Take(pageSize).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Account?> UpdateAsync(Guid id, Account account)
        {
            using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingAccount = await dbContext.Accounts.FindAsync(id);
                    var CustomerToUpdate = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == existingAccount.ContactNumber && x.CustomerName == existingAccount.CustomerName);
                    //var dtoProperties = typeof(Account).GetProperties();

                    //foreach (var item in dtoProperties)
                    //{
                    //    Console.WriteLine("dtoProperties : " + item);
                    //}

                    //foreach (var propertyInfo in dtoProperties)
                    //{
                    //    var dtoValue = propertyInfo.GetValue(account);

                    //    if (dtoValue != null)
                    //    {
                    //        var accountyProperty = typeof(Account).GetProperty(propertyInfo.Name);

                    //        if (accountyProperty != null)
                    //        {
                    //            accountyProperty.SetValue(existingAccount, dtoValue);
                    //            Console.WriteLine("Account Property  : " + accountyProperty);
                    //        }
                    //    }
                    //}
                    if (existingAccount == null)
                    {
                        throw new CustomerExceptionHandler("Account doesn't exist", 400);
                    }
                    if (account.Balance != 0)
                    {
                        existingAccount.Balance = account.Balance;
                    }
                    if (!string.IsNullOrWhiteSpace(account.ContactNumber))
                    {
                        var customer = await dbContext.Customers.FirstOrDefaultAsync(x=>x.ContactNumber == account.ContactNumber);
                        if (customer == null)
                        {
                            
                            CustomerToUpdate.ContactNumber = account.ContactNumber;
                            existingAccount.ContactNumber = account.ContactNumber;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(account.Address))
                    {
                        CustomerToUpdate.Address = account.Address;
                        existingAccount.Address = account.Address;
                    }
                    //if ( account.AccountTypeId==null && ( 3 < existingAccount.AccountTypeId || existingAccount.AccountTypeId < 1))
                    //{
                    //    throw new CustomerExceptionHandler("AccountTypeId not matched", 400);
                    //}
                    if(account.AccountTypeId != 0 && (3 >= existingAccount.AccountTypeId && existingAccount.AccountTypeId >= 1))
                    {
                        
                        if (3 >= account.AccountTypeId && account.AccountTypeId >= 1)
                        {
                            existingAccount.AccountTypeId = account.AccountTypeId;
                        }
                    }
                    else if((3 <= existingAccount.AccountTypeId && existingAccount.AccountTypeId <= 1))
                    {
                        throw new CustomerExceptionHandler("AccountTypeId not matched", 400);
                    }
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return existingAccount;
                }
                catch (Exception ex)
                {
                    transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<Account?> DeleteAsync(Guid id)
        {
            using (IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var account = await dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == id);
                    //if (account == null)
                    //{
                    //    throw new CustomerExceptionHandler("Account doesn't exist", 400);
                    //}
                    dbContext.Accounts.Remove(account);
                    var customer = await dbContext.Customers.FirstOrDefaultAsync(x => x.ContactNumber == account.ContactNumber && x.CustomerName == account.CustomerName);
                    if (customer != null)
                    {
                        customer.AccountNumber = null;
                        customer = await customerRepo.UpdateAsync(customer.Id, customer);
                        
                    }
                    //var customerAccount = await dbContext.CustomersAccounts.Where(x => x.AccountId == account.Id).ToListAsync();
                    //if (customerAccount != null && customerAccount.Any())
                    //{
                    //    dbContext.CustomersAccounts.RemoveRange(customerAccount);
                    //}
                    var transactions = await dbContext.AccountTransactions.Where(x => x.AccountId == account.Id).ToListAsync();
                    if (transactions != null && transactions.Any())
                    {
                        dbContext.AccountTransactions.RemoveRange(transactions);
                    }
                    await dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return account;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            try
            {
                var account = await dbContext.Accounts.FindAsync(id);
                if (account == null)
                {
                    throw new CustomerExceptionHandler("Account doesn't exist", 400);
                }
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Customer>> GetAllCustomerByAccountIdAsync(Guid id)
        {
            try
            {
                var customerIds = await dbContext.CustomersAccounts.Where(x => x.AccountId == id).Select(ca => ca.CustomerId).ToListAsync();
                var customers = await dbContext.Customers.Where(x=> customerIds.Contains(x.Id)).ToListAsync();
                return customers;
            }
            catch(CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Transaction>> GetAllTransactionByAccountIdAsync(Guid id)
        {
            try
            {
                var transactionIds = await dbContext.AccountTransactions.Where(x => x.AccountId == id).Select(ca => ca.TransactionId).ToListAsync();
                var transactions = await dbContext.Transactions.Where(x => transactionIds.Contains(x.Id)).ToListAsync();
                return transactions;
            }
            catch (CustomerExceptionHandler ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }
        
    }
}
