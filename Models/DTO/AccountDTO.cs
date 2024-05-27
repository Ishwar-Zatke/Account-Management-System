using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class AccountDTO
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string AccountNumber { get; set; }
        [Required]
        public decimal Balance { get; set; }

        //[Required]
        //public Guid CustomerId { get; set; }

        [Required]
        public int AccountTypeId { get; set; }


        //public Guid TransactionId { get; set; }


        //For Customer Creation
        public string? CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string? Address { get; set; }
    }
}
