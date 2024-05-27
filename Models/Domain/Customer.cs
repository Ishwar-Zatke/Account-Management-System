using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }

        [ForeignKey("Account")]
        public string? AccountNumber { get; set; }

        public List<Account> Account { get; set; }


    }
}
