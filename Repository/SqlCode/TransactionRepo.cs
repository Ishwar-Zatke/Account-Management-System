using Account_Management.Data;
using Account_Management.Middleware;
using Account_Management.Models.Domain;
using Account_Management.Models.DTO;
using Account_Management.Repository.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Security.Principal;

namespace Account_Management.Repository.SqlCode
{
    public class TransactionRepo: ITransactionRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public TransactionRepo(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }


        public async Task<List<Transaction>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            try
            {
                var transaction = dbContext.Transactions
                .Include("TransactionType")
                .Include("Account")
                .AsQueryable();
                if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
                {
                    if (filterOn.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.Id.Equals(filterQuery));
                    }
                    if (filterOn.Equals("FromAccountId", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.FromAccountId.Equals(filterQuery));
                    }
                    if (filterOn.Equals("ToAccountId", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.ToAccountId.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.Amount.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("Date", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.Date.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("TransactionTypeId", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = transaction.Where(x => x.TransactionTypeId.Equals(filterQuery));
                    }
                }

                //Sorting
                if (string.IsNullOrWhiteSpace(sortBy) == false)
                {
                    if (sortBy.Equals("FromAccountId", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = isAscending ? transaction.OrderBy(x => x.FromAccountId) : transaction.OrderByDescending(x => x.FromAccountId);
                    }
                    else if (sortBy.Equals("Amount", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = isAscending ? transaction.OrderBy(x => x.Amount) : transaction.OrderByDescending(x => x.Amount);
                    }
                    else if (sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = isAscending ? transaction.OrderBy(x => x.Id) : transaction.OrderByDescending(x => x.Id);
                    }
                    else if (sortBy.Equals("TransactionTypeId", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = isAscending ? transaction.OrderBy(x => x.TransactionTypeId) : transaction.OrderByDescending(x => x.TransactionTypeId);
                    }
                    else if (sortBy.Equals("Date", StringComparison.OrdinalIgnoreCase))
                    {
                        transaction = isAscending ? transaction.OrderBy(x => x.Date) : transaction.OrderByDescending(x => x.Date);
                    }
                }


                //Pagination
                var skipResults = (pageNumber - 1) * pageSize;
                return await transaction.Skip(skipResults).Take(pageSize).ToListAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Transaction?> GetById(Guid id)
        {
            try
            {
                var transaction = await dbContext.Transactions.FindAsync(id);
                //if (transaction == null)
                //{
                //    throw new CustomerExceptionHandler("Transaction Doesn't Exist", 400);
                //}
                return transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Transaction>?> GetAllByAccountIdAsync(Guid AccountId)
        {
            try
            {
                var transaction = await dbContext.Transactions.Where(x => x.FromAccountId == AccountId).ToListAsync();
                //if (transaction == null)
                //{
                //    throw new CustomerExceptionHandler("Transaction Doesn't Exist for the given Account", 400);
                //}
                return transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            using (IDbContextTransaction Transaction = await dbContext.Database.BeginTransactionAsync())
            {
                try
                {

                    var FromAccount = await dbContext.Accounts.FindAsync(transaction.FromAccountId);
                    var ToAccount = await dbContext.Accounts.FindAsync(transaction.ToAccountId);
                    if(FromAccount == null || ToAccount == null)
                    {
                        throw new CustomerExceptionHandler("Provide Correct Account Details", 400);
                    }
                    Guid? accountId = transaction.FromAccountId;
                    if (transaction.TransactionTypeId == 1)
                    {
                        ToAccount.Balance = ToAccount.Balance + transaction.Amount;
                        accountId = transaction.ToAccountId;
                    }
                    else if (transaction.TransactionTypeId == 2)
                    {
                        FromAccount.Balance = FromAccount.Balance - transaction.Amount;
                        accountId = transaction.FromAccountId;
                    }
                    else if (transaction.TransactionTypeId == 3)
                    {
                        FromAccount.Balance = FromAccount.Balance - transaction.Amount;
                        ToAccount.Balance = ToAccount.Balance + transaction.Amount;
                    }
                    else
                    {
                        throw new CustomerExceptionHandler("Transaction Type doesn't exist", 400);
                    }

                    await dbContext.Transactions.AddAsync(transaction);
                    await dbContext.SaveChangesAsync();
                    var accountTransactions = new AccountTransaction()
                    {
                        Id = Guid.NewGuid(),
                        TransactionId = transaction.Id,
                        AccountId = (Guid)accountId,
                        TransactionTypeId = transaction.TransactionTypeId
                    };
                    var accountTransaction = mapper.Map<AccountTransaction>(accountTransactions);
                    await dbContext.AccountTransactions.AddAsync(accountTransaction);
                    await dbContext.SaveChangesAsync();
                    await Transaction.CommitAsync();
                    return transaction;
                }
                catch (Exception ex)
                {
                    await Transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
