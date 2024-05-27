using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.DTO
{
    public class AddTransactionRequestDTO
    {
        [Required(ErrorMessage = "Account Id required")]
        public Guid FromAccountId { get; set; }

        [Required(ErrorMessage = "Account Id required")]
        public Guid ToAccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Date Time Required")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "TransactionTypeId Required")]
        public int TransactionTypeId { get; set; }

    }
}
