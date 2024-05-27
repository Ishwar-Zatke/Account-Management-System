using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class UpdateCustomerRequestDTO
    {
        
        public string? CustomerName { get; set; }

        [RegularExpression(@"^(?:\+\d{12}|\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string? ContactNumber { get; set; }
        
        public string? Address { get; set; }

        [MinLength(5, ErrorMessage = "Length of Account Should be 5")]
        [MaxLength(5, ErrorMessage = "Length of Account Should be 5")]
        public string? AccountNumber { get; set; }

        

    }
}
