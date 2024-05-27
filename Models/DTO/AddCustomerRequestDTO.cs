using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Account_Management.Models.Domain;

namespace Account_Management.Models.DTO
{
    public class AddCustomerRequestDTO
    {
      
        [Required(ErrorMessage = "Customer Name Required")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^(?:\+\d{12}|\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage =" Customer's Address needs to be added")]
        public string Address { get; set; }
        
        [MinLength(5, ErrorMessage = "Length of Account Should be 5")]
        [MaxLength(5, ErrorMessage = "Length of Account Should be 5")]
        public string? AccountNumber { get; set; }


    }
}
