using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
       
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("FromAccountId")]
        public Account FromAccount { get; set; }

        [ForeignKey("ToAccountId")]
        public Account ToAccount { get; set; }

        [ForeignKey("TransactionType")]
        public int TransactionTypeId { get; set; }
        public TransactionType TransactionType { get; set; }

        public List<Account> Account {  get; set; }
    }
}
