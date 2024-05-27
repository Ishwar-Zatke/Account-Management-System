using Account_Management.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Account_Management.Models.DTO
{
    public class RegisterRequestDTO
    {

        [Required]
        public string? userId { get; set; }
        [Required]
        public string? username { get; set; }
        [Required]
        public string? firstname { get; set; }
        [Required]
        public string? lastname { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? password { get; set; }
        public string Role { get; set; }
    }
}
