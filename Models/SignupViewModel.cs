using System.ComponentModel.DataAnnotations;

namespace identity.Models
{
    public class SignupViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email address is invalid")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Password is incorrect")]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Department { get; set; }
    }
}