using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class CustomersAccountDTO
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDTO Customer { get; set; }
        public AccountDTO Account { get; set; }
    }
}
