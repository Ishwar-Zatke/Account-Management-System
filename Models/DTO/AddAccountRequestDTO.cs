using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class AddAccountRequestDTO
    {
        //[Required(ErrorMessage = "Account Number is required.")]
        //[MinLength(5, ErrorMessage = "Length of Account Should be 5")]
        //[MaxLength(5, ErrorMessage = "Length of Account Should be 5")]
        //public string AccountNumber { get; set; }

        [Required(ErrorMessage= "Balance to be entered")]
        public decimal Balance { get; set; }
        //[Required]
        //public Guid CustomerId { get; set; }
        [Required(ErrorMessage = "AccountTypeId Required")]
        public int AccountTypeId { get; set; }
        //[Required]
        //public Guid TransactionId { get; set; }

        //For Customer Creation
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Contact Number is required.")]
        [RegularExpression(@"^(?:\+\d{12}|\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; }

        public string? Address { get; set; }
    }
}
