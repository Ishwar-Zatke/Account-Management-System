using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class AccountTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Account Account { get; set; }
        public Guid TransactionId { get; set; }
        public int TransactionTypeId { get; set; }
        public Transaction Transaction { get; set; }

        
    }
}
