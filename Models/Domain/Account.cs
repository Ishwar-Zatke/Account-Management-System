using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0;

        //[ForeignKey("Customer")]
        //public Guid CustomerId { get; set; }

        [ForeignKey("AccountType")]
        public int AccountTypeId { get; set; }

        public List<Transaction> Transaction { get; set; }
        public List<Customer> Customer { get; set; }

        public AccountType AccountType { get; set; }

        //For Customer
        [ForeignKey("Customer")]
        public string? CustomerName { get; set; }
        [ForeignKey("Customer")]
        public string ContactNumber { get; set; }
        [ForeignKey("Customer")]
        public string? Address { get; set; }

    }
}
