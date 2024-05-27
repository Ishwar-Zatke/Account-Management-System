using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.Domain
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string? userId { get; set; }
        public string? username { get; set; }
        public string? firstname { get; set; }
        public string? lastname { get; set;}
        public string? email { get; set; }
        public string? password { get; set; }

        public string roleType { get; set; }

        [ForeignKey("Role")]
        public Guid? roleId { get; set; }
        public Role Role { get; set; }
    }
}
