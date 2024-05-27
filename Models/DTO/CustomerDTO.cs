using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.DTO
{
    public class CustomerDTO
    {
        [Required]
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? AccountNumber { get; set; }


    }
}
