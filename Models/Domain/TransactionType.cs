using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class TransactionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
