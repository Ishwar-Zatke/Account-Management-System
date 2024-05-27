using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class UpdateAccountRequestDTO
    {

        public decimal? Balance { get; set; }

        //public Guid? CustomerId { get; set; }
        public int? AccountTypeId { get; set; }

        //public Guid? TransactionId { get; set; }

        //For Customer Creation
        [RegularExpression(@"^(?:\+\d{12}|\d{10})$", ErrorMessage = "Invalid phone number format.")]
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
    }
}
