using Account_Management.Models.Domain;
using Microsoft.EntityFrameworkCore;
namespace Account_Management.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<AccountTransaction> AccountTransactions { get; set; }
        public DbSet<CustomersAccount> CustomersAccounts { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Unique ContactNumber for everyCustomer
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.ContactNumber)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(c => c.AccountNumber)
                .IsUnique();

            modelBuilder.Entity<CustomersAccount>()
                .HasKey(ca => new { ca.CustomerId, ca.AccountId });
            modelBuilder.Entity<CustomersAccount>()
                .HasOne(at => at.Account)
                .WithMany()
                .HasForeignKey(at => at.AccountId);
            modelBuilder.Entity<CustomersAccount>()
                .HasOne(at => at.Customer)
                .WithMany()
                .HasForeignKey(at => at.CustomerId);

            modelBuilder.Entity<AccountTransaction>()
                .HasKey(at => new { at.AccountId, at.TransactionId });

            modelBuilder.Entity<AccountTransaction>()
                .HasOne(at => at.Account)
                .WithMany()
                .HasForeignKey(at => at.AccountId);

            modelBuilder.Entity<AccountTransaction>()
                .HasOne(at => at.Transaction)
                .WithMany()
                .HasForeignKey(at => at.TransactionId);



            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.FromAccount)
                .WithMany()
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ToAccount)
                .WithMany()
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            var AccountTypes = new List<AccountType>()
            {
                new AccountType()
                {
                    Id = 1,
                    Name = "Savings"
                },
                new AccountType()
                {
                    Id = 2,
                    Name = "Current"
                },
                new AccountType()
                {
                    Id = 3,
                    Name = "Joint"
                }
            };
            modelBuilder.Entity<AccountType>().HasData(AccountTypes);
            var TransactionTypes = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Id = 1,
                    Name = "Deposit"
                },
                new TransactionType()
                {
                    Id = 2,
                    Name = "Withdrawal"
                },
                new TransactionType()
                {
                    Id = 3,
                    Name = "Transfer"
                }
            };
            modelBuilder.Entity<TransactionType>().HasData(TransactionTypes);
            var readerRoleId = Guid.NewGuid();
            var writerRoleId = Guid.NewGuid();
            var Roles = new List<Role>()
            {
                new Role()
                {
                    Id = readerRoleId,
                    Name = "Reader"
                },
                new Role()
                {
                    Id = writerRoleId,
                    Name = "Writer"
                }
            };
            modelBuilder.Entity<Role>().HasData(Roles);
        }       
    }
}
