using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.Domain
{
    public class AccountType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
