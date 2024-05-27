using System.ComponentModel.DataAnnotations;

namespace Account_Management.Models.Domain
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
