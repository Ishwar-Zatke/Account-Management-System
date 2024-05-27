using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class AccountTransactionDTO
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public AccountDTO Account { get; set; }
        public Guid TransactionId { get; set; }
        public TransactionDTO Transaction { get; set; }
    }
}
